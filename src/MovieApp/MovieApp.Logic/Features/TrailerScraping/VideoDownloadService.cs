using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApp.Logic.Features.TrailerScraping
{
    /// <summary>
    /// Downloads YouTube videos as MP4 files using yt-dlp.
    /// Owner: Andrei.
    /// </summary>
    public class VideoDownloadService : IVideoDownloadService
    {
        private const int ProcessTimeoutMinutes = 5;
        private const int DefaultMaxDurationSeconds = 60;
        private const int SuccessExitCode = 0;

        private const string YtDlpExecutableName = "yt-dlp.exe";
        private const string FfmpegExecutableName = "ffmpeg.exe";
        private const string AppDataFolderName = "MeioAI";
        private const string VideosFolderName = "Videos";
        private const string MicrosoftFolderName = "Microsoft";
        private const string WinGetFolderName = "WinGet";
        private const string LinksFolderName = "Links";
        private const string Mp4FileFormat = "{0}.mp4";

        private const string ErrorProcessStartFailed = "Failed to start yt-dlp process";
        private const string ErrorProcessTimeout = "yt-dlp process timed out after 5 minutes";
        private const string ErrorExitCodeFormat = "yt-dlp exit code {0}";
        private const string ErrorProcessExceptionFormat = "yt-dlp process error: {0}";

        private readonly string downloadFolder;
        private string ytDlpPath = YtDlpExecutableName;
        private string ffmpegPath = FfmpegExecutableName;
        private bool isInitialized;

        public VideoDownloadService(string? downloadFolder = null)
        {
            this.downloadFolder = downloadFolder
                ?? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    AppDataFolderName,
                    VideosFolderName);

            Directory.CreateDirectory(this.downloadFolder);
        }

        public string? LastError { get; private set; }

        public Task EnsureDependenciesAsync()
        {
            if (this.isInitialized) return Task.CompletedTask;

            string wingetLinks = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                MicrosoftFolderName, WinGetFolderName, LinksFolderName);

            string wingetYtDlp = Path.Combine(wingetLinks, YtDlpExecutableName);
            string wingetFfmpeg = Path.Combine(wingetLinks, FfmpegExecutableName);

            if (File.Exists(wingetYtDlp) && File.Exists(wingetFfmpeg))
            {
                this.ytDlpPath = wingetYtDlp;
                this.ffmpegPath = wingetFfmpeg;
                this.isInitialized = true;
                return Task.CompletedTask;
            }

            // Explicitly resolve absolute paths to guarantee yt-dlp can find ffmpeg
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string localYtDlp = Path.Combine(currentDir, YtDlpExecutableName);
            string localFfmpeg = Path.Combine(currentDir, FfmpegExecutableName);

            if (File.Exists(localYtDlp) && File.Exists(localFfmpeg))
            {
                this.ytDlpPath = localYtDlp;
                this.ffmpegPath = localFfmpeg;
            }

            this.isInitialized = true;
            return Task.CompletedTask;
        }

        public async Task<string?> DownloadVideoAsMp4Async(string youtubeUrl, int maxDurationSeconds = DefaultMaxDurationSeconds)
        {
            await this.EnsureDependenciesAsync();

            // 1. Generate a totally unique ID so we don't have to parse console text!
            string uniqueFileName = Guid.NewGuid().ToString("N");
            string outputTemplate = Path.Combine(this.downloadFolder, $"{uniqueFileName}.%(ext)s");
            string expectedFinalFile = Path.Combine(this.downloadFolder, $"{uniqueFileName}.mp4");

            string ffmpegArgs = "-nostdin -y";
            if (maxDurationSeconds > 0)
            {
                ffmpegArgs += $" -t {maxDurationSeconds}";
            }

            string processArguments = $"--force-overwrites --no-playlist --no-progress --ffmpeg-location \"{this.ffmpegPath}\" -f \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best\" --merge-output-format mp4 --postprocessor-args \"ffmpeg:{ffmpegArgs}\" -o \"{outputTemplate}\" \"{youtubeUrl}\"";

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = this.ytDlpPath,
                    Arguments = processArguments,
                    UseShellExecute = false,
                    // 2. THE FIX: We turn OFF all pipe redirection. 
                    // C# will no longer deadlock waiting for ffmpeg to close its inherited streams!
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    CreateNoWindow = true,
                };

                using Process downloadProcess = Process.Start(processStartInfo);
                if (downloadProcess == null)
                {
                    this.LastError = ErrorProcessStartFailed;
                    return null;
                }

                using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(ProcessTimeoutMinutes));
                try
                {
                    // This will now perfectly complete the second yt-dlp finishes.
                    await downloadProcess.WaitForExitAsync(cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    try { downloadProcess.Kill(entireProcessTree: true); } catch { }
                    this.LastError = ErrorProcessTimeout;
                    return null;
                }

                if (downloadProcess.ExitCode != SuccessExitCode)
                {
                    this.LastError = string.Format(ErrorExitCodeFormat, downloadProcess.ExitCode);
                    return null;
                }

                // 3. We instantly know where the file is because we assigned the GUID
                if (File.Exists(expectedFinalFile))
                {
                    this.LastError = null;
                    return expectedFinalFile;
                }

                this.LastError = "yt-dlp finished but the output MP4 file was not found.";
                return null;
            }
            catch (Exception exception)
            {
                this.LastError = string.Format(ErrorProcessExceptionFormat, exception.Message);
                return null;
            }
        }

        public string GetExpectedFilePath(string videoId)
        {
            return Path.Combine(this.downloadFolder, string.Format(Mp4FileFormat, videoId));
        }
    }
}

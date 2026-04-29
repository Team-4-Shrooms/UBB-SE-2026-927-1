namespace MovieApp.Features.ReelsEditing.Services
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using MovieApp.Logic.Models;

    /// <summary>
    /// Service responsible for processing video and audio files using FFmpeg.
    /// </summary>
    public class VideoProcessingService : IVideoProcessingService
    {
        private const int BaseWidth = 1920;
        private const int BaseHeight = 1080;
        private const int MinimumCropDimension = 1;
        private const int EmptyCoordinate = 0;
        private const double MaxStartOffsetSeconds = 300.0;
        private const double VolumePercentageDivisor = 100.0;
        private const double MaxVolumeMultiplier = 2.0;
        private const double MinimumVolumeMultiplier = 0.0;
        private const double DefaultAudioDurationSeconds = 30.0;
        private const double MinimumAudioDurationSeconds = 1.0;
        private const double AudioStartOffsetMarginSeconds = 0.25;
        private const int SuccessExitCode = 0;

        private const string FfmpegExecutableName = "ffmpeg.exe";
        private const string FfprobeExecutableName = "ffprobe.exe";
        private const string FfmpegFallbackName = "ffmpeg";
        private const string FfprobeFallbackName = "ffprobe";
        private const string TempCropFileSuffix = "_crop_tmp_";
        private const string TempMusicFileSuffix = "_music_tmp_";
        private const string FinalCroppedSuffix = "_cropped_";
        private const string FinalWithMusicSuffix = "_withmusic_";
        private const string TimestampFormat = "yyyyMMddHHmmssfff";

        private const string CropFilterFormat = "crop=iw*{0:0.######}:ih*{1:0.######}:iw*{2:0.######}:ih*{3:0.######}";
        private const string FfmpegCropArgumentsFormat = "-hide_banner -loglevel error -i \"{0}\" -vf \"{1}\" -c:v libx264 -preset veryfast -crf 20 -c:a copy -movflags +faststart -y \"{2}\"";
        private const string DurationFilterFormat = ",atrim=duration={0}";
        private const string VolumeFilterFormat = ",volume={0}";
        private const string AudioFilterComplexFormat = "[1:a]aresample=async=1:first_pts=0{0}{1},apad[aout]";
        private const string FfmpegMusicArgumentsFormat = "-hide_banner -loglevel error -i \"{0}\" -stream_loop -1 -ss {1} -i \"{2}\" -filter_complex \"{3}\" -map 0:v:0 -map \"[aout]\" -c:v copy -c:a aac -b:a 192k -movflags +faststart -shortest -y \"{4}\"";
        private const string FfprobeDurationArgumentsFormat = "-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{0}\"";

        private const string ErrorCropOutputMissing = "FFmpeg did not produce the cropped output file.";
        private const string ErrorMusicOutputMissing = "FFmpeg did not produce the merged-audio output file.";
        private const string ErrorMusicFileNotFoundFormat = "Music file not found: {0}";
        private const string ErrorFfmpegStartFailed = "Failed to start ffmpeg. Ensure ffmpeg is installed and available.";
        private const string ErrorFfmpegTimeout = "ffmpeg timed out after 5 minutes.";
        private const string ErrorFfmpegExitCodeFormat = "ffmpeg exited with code {0}:{1}{2}{1}{3}";

        private const string JsonKeyX = "x";
        private const string JsonKeyY = "y";
        private const string JsonKeyWidth = "width";
        private const string JsonKeyHeight = "height";
        private const string InvariantNumberFormat = "0.###";

        private static TimeSpan ffmpegTimeout = TimeSpan.FromMinutes(5);
        private static readonly string LocalFfmpegPath = Path.Combine(AppContext.BaseDirectory, FfmpegExecutableName);
        private static readonly string LocalFfprobePath = Path.Combine(AppContext.BaseDirectory, FfprobeExecutableName);
        private static Action<string>? cropOutputPostProcessHook;

        private readonly IAudioLibraryRepository audioLibrary;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoProcessingService"/> class.
        /// </summary>
        /// <param name="audioLibrary">The audio library service used to fetch background music.</param>
        public VideoProcessingService(IAudioLibraryRepository audioLibrary)
        {
            this.audioLibrary = audioLibrary;
        }

        /// <summary>
        /// Applies a crop to the specified video based on the provided JSON metadata.
        /// </summary>
        /// <param name="videoPath">The path or URL to the source video file.</param>
        /// <param name="cropDataJson">The JSON string containing the crop dimensions and coordinates.</param>
        /// <returns>A task containing the path to the cropped video file.</returns>
        public async Task<string> ApplyCropAsync(string videoPath, string cropDataJson)
        {
            string sourcePath = ResolveMediaInput(videoPath);
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            {
                return videoPath;
            }

            // Replace the incorrect Tuple<int> declaration with a deconstruction of the tuple returned by ReadCropData
            (int cropX, int cropY, int cropWidth, int cropHeight) = ReadCropData(cropDataJson);

            if (cropX == EmptyCoordinate && cropY == EmptyCoordinate && cropWidth == BaseWidth && cropHeight == BaseHeight)
            {
                return videoPath;
            }

            double widthRatio = (double)cropWidth / BaseWidth;
            double heightRatio = (double)cropHeight / BaseHeight;
            double xRatio = (double)cropX / BaseWidth;
            double yRatio = (double)cropY / BaseHeight;

            string cropFilter = string.Format(
                CultureInfo.InvariantCulture,
                CropFilterFormat,
                widthRatio,
                heightRatio,
                xRatio,
                yRatio);

            string directory = Path.GetDirectoryName(sourcePath) !;
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(sourcePath);
            string extension = Path.GetExtension(sourcePath);
            string tempPath = Path.Combine(directory, $"{fileNameWithoutExt}{TempCropFileSuffix}{Guid.NewGuid():N}{extension}");

            string ffmpegArguments = string.Format(FfmpegCropArgumentsFormat, sourcePath, cropFilter, tempPath);

            await RunFfmpegAsync(ffmpegArguments, directory);
            cropOutputPostProcessHook?.Invoke(tempPath);

            if (!File.Exists(tempPath))
            {
                throw new InvalidOperationException(ErrorCropOutputMissing);
            }

            return FinalizeProcessedFile(sourcePath, tempPath, FinalCroppedSuffix);
        }

        /// <summary>
        /// Merges an audio track into the specified video file.
        /// </summary>
        /// <param name="videoPath">The path or URL to the source video file.</param>
        /// <param name="musicTrackId">The unique identifier of the background music track.</param>
        /// <param name="startOffsetSec">The start time offset for the audio track, in seconds.</param>
        /// <param name="musicDurationSec">The duration of the audio to play, in seconds.</param>
        /// <param name="musicVolumePercent">The volume level of the music as a percentage.</param>
        /// <returns>A task containing the path to the video file with merged audio.</returns>
        public async Task<string> MergeAudioAsync(
            string videoPath,
            int musicTrackId,
            double startOffsetSec,
            double musicDurationSec,
            double musicVolumePercent)
        {
            string sourcePath = ResolveMediaInput(videoPath);
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            {
                return videoPath;
            }

            MusicTrack track = await this.audioLibrary.GetTrackByIdAsync(musicTrackId);
            if (track == null || string.IsNullOrWhiteSpace(track.AudioUrl))
            {
                return videoPath;
            }

            string audioInput = ResolveMediaInput(track.AudioUrl);
            if (!IsHttpUrl(audioInput) && !File.Exists(audioInput))
            {
                throw new FileNotFoundException(string.Format(ErrorMusicFileNotFoundFormat, audioInput));
            }

            double safeStart = Math.Clamp(startOffsetSec, EmptyCoordinate, MaxStartOffsetSeconds);
            double safeVolume = Math.Clamp(musicVolumePercent / VolumePercentageDivisor, MinimumVolumeMultiplier, MaxVolumeMultiplier);

            string directory = Path.GetDirectoryName(sourcePath) !;

            double? videoDuration = await TryGetMediaDurationSecondsAsync(sourcePath, directory: directory);
            double targetDuration = videoDuration.HasValue && videoDuration.Value > 0
                ? videoDuration.Value
                : (musicDurationSec > 0 ? musicDurationSec : DefaultAudioDurationSeconds);

            double? probedAudioDuration = await TryGetMediaDurationSecondsAsync(audioInput, directory: directory);
            if (!probedAudioDuration.HasValue && (double)track.DurationSeconds > MinimumAudioDurationSeconds)
            {
                probedAudioDuration = (double)track.DurationSeconds;
            }

            if (probedAudioDuration.HasValue && probedAudioDuration.Value > 0)
            {
                double audioDuration = probedAudioDuration.Value;
                if (safeStart >= audioDuration - AudioStartOffsetMarginSeconds)
                {
                    safeStart = EmptyCoordinate;
                }

                double availableAfterStart = audioDuration - safeStart;
                if (availableAfterStart < MinimumAudioDurationSeconds)
                {
                    safeStart = EmptyCoordinate;
                }
            }

            string durationFilter = string.Format(DurationFilterFormat, ToInvariantNumber(targetDuration));
            string volumeFilter = string.Format(VolumeFilterFormat, ToInvariantNumber(safeVolume));

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(sourcePath);
            string extension = Path.GetExtension(sourcePath);
            string tempPath = Path.Combine(directory, $"{fileNameWithoutExt}{TempMusicFileSuffix}{Guid.NewGuid():N}{extension}");

            string filterComplex = string.Format(AudioFilterComplexFormat, durationFilter, volumeFilter);

            string ffmpegArguments = string.Format(
                FfmpegMusicArgumentsFormat,
                sourcePath,
                ToInvariantNumber(safeStart),
                audioInput,
                filterComplex,
                tempPath);

            await RunFfmpegAsync(ffmpegArguments, directory);

            if (!File.Exists(tempPath))
            {
                throw new InvalidOperationException(ErrorMusicOutputMissing);
            }

            return FinalizeProcessedFile(sourcePath, tempPath, FinalWithMusicSuffix);
        }

        /// <summary>
        /// Attempts to get the media duration in seconds using ffprobe.
        /// </summary>
        /// <param name="mediaInput">The path or URL to the media file.</param>
        /// <param name="directory">The working directory for the process.</param>
        /// <returns>The duration in seconds, or null if it fails.</returns>
        private static async Task<double?> TryGetMediaDurationSecondsAsync(string mediaInput, string directory)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = ResolveFfprobePath(),
                Arguments = string.Format(FfprobeDurationArgumentsFormat, mediaInput),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = directory,
            };

            using Process process = Process.Start(processStartInfo);
            if (process == null)
            {
                return null;
            }

            Task<string> standardOutputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> standardErrorTask = process.StandardError.ReadToEndAsync();

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(ffmpegTimeout);
            try
            {
                await process.WaitForExitAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch
                {
                    // Ignore errors during kill
                }

                return null;
            }

            string standardOutput = (await standardOutputTask).Trim();
            _ = await standardErrorTask;

            if (process.ExitCode != SuccessExitCode || string.IsNullOrWhiteSpace(standardOutput))
            {
                return null;
            }

            if (double.TryParse(standardOutput, NumberStyles.Float, CultureInfo.InvariantCulture, out Double duration) &&
                duration > 0)
            {
                return duration;
            }

            return null;
        }

        /// <summary>
        /// Replaces the source file with the processed temporary file and cleans up.
        /// </summary>
        /// <param name="sourcePath">The original source path.</param>
        /// <param name="tempPath">The temporary processed file path.</param>
        /// <param name="fallbackSuffix">The suffix to use if file replacement fails.</param>
        /// <returns>The final file path.</returns>
        private static string FinalizeProcessedFile(string sourcePath, string tempPath, string fallbackSuffix)
        {
            string directory = Path.GetDirectoryName(sourcePath) !;
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(sourcePath);
            string extension = Path.GetExtension(sourcePath);
            string fallbackPath = Path.Combine(
                directory,
                $"{fileNameWithoutExt}{fallbackSuffix}{DateTime.UtcNow.ToString(TimestampFormat)}{extension}");

            try
            {
                File.Move(tempPath, sourcePath, overwrite: true);
                return sourcePath;
            }
            catch (IOException)
            {
                File.Move(tempPath, fallbackPath, overwrite: true);
                return fallbackPath;
            }
            catch (UnauthorizedAccessException)
            {
                File.Move(tempPath, fallbackPath, overwrite: true);
                return fallbackPath;
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    try
                    {
                        File.Delete(tempPath);
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
            }
        }

        /// <summary>
        /// Runs an FFmpeg process with the specified arguments.
        /// </summary>
        /// <param name="arguments">The arguments to pass to FFmpeg.</param>
        /// <param name="workingDirectory">The working directory for the process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task RunFfmpegAsync(string arguments, string workingDirectory)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = ResolveFfmpegPath(),
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory,
            };

            using Process process = Process.Start(processStartInfo)
                ?? throw new InvalidOperationException(ErrorFfmpegStartFailed);

            Task<string> standardOutputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> standardErrorTask = process.StandardError.ReadToEndAsync();

            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(ffmpegTimeout);
            try
            {
                await process.WaitForExitAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch
                {
                    // Ignore errors during kill
                }

                throw new InvalidOperationException(ErrorFfmpegTimeout);
            }

            string standardOutput = await standardOutputTask;
            string standardError = await standardErrorTask;

            if (process.ExitCode != SuccessExitCode)
            {
                throw new InvalidOperationException(string.Format(
                    ErrorFfmpegExitCodeFormat,
                    process.ExitCode,
                    Environment.NewLine,
                    standardError,
                    standardOutput));
            }
        }

        /// <summary>
        /// Resolves the path to the FFmpeg executable.
        /// </summary>
        /// <returns>The path or command for FFmpeg.</returns>
        private static string ResolveFfmpegPath()
        {
            if (File.Exists(LocalFfmpegPath))
            {
                return LocalFfmpegPath;
            }

            return FfmpegFallbackName;
        }

        /// <summary>
        /// Resolves the path to the ffprobe executable.
        /// </summary>
        /// <returns>The path or command for ffprobe.</returns>
        private static string ResolveFfprobePath()
        {
            if (File.Exists(LocalFfprobePath))
            {
                return LocalFfprobePath;
            }

            return FfprobeFallbackName;
        }

        /// <summary>
        /// Resolves a media input string, converting local URIs to file paths.
        /// </summary>
        /// <param name="value">The media input string.</param>
        /// <returns>The resolved path or URL.</returns>
        private static string ResolveMediaInput(string value)
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out Uri uri) && uri.IsFile)
            {
                return uri.LocalPath;
            }

            return value;
        }

        /// <summary>
        /// Checks if a string is an HTTP or HTTPS URL.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns>True if it is an HTTP/HTTPS URL, otherwise false.</returns>
        private static bool IsHttpUrl(string value)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out Uri uri))
            {
                return false;
            }

            return string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Converts a double value to an invariant culture string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The formatted string.</returns>
        private static string ToInvariantNumber(double value)
            => value.ToString(InvariantNumberFormat, CultureInfo.InvariantCulture);

        /// <summary>
        /// Parses JSON crop data into its constituent coordinates and dimensions.
        /// </summary>
        /// <param name="cropDataJson">The JSON string to parse.</param>
        /// <returns>A tuple containing the X, Y, Width, and Height values.</returns>
        private static (int CropX, int CropY, int CropWidth, int CropHeight) ReadCropData(string cropDataJson)
        {
            if (string.IsNullOrWhiteSpace(cropDataJson))
            {
                return (EmptyCoordinate, EmptyCoordinate, BaseWidth, BaseHeight);
            }

            using JsonDocument jsonDocument = JsonDocument.Parse(cropDataJson);
            JsonElement rootElement = jsonDocument.RootElement;

            int cropX = ReadInt(rootElement, JsonKeyX, EmptyCoordinate);
            int cropY = ReadInt(rootElement, JsonKeyY, EmptyCoordinate);
            int cropWidth = ReadInt(rootElement, JsonKeyWidth, BaseWidth);
            int cropHeight = ReadInt(rootElement, JsonKeyHeight, BaseHeight);

            cropX = Math.Clamp(cropX, EmptyCoordinate, BaseWidth - MinimumCropDimension);
            cropY = Math.Clamp(cropY, EmptyCoordinate, BaseHeight - MinimumCropDimension);
            cropWidth = Math.Clamp(cropWidth, MinimumCropDimension, BaseWidth - cropX);
            cropHeight = Math.Clamp(cropHeight, MinimumCropDimension, BaseHeight - cropY);

            return (cropX, cropY, cropWidth, cropHeight);
        }

        /// <summary>
        /// Reads an integer from a JSON element with a fallback value.
        /// </summary>
        /// <param name="rootElement">The root JSON element.</param>
        /// <param name="propertyName">The property name to read.</param>
        /// <param name="fallbackValue">The fallback value if parsing fails.</param>
        /// <returns>The parsed integer or the fallback value.</returns>
        private static int ReadInt(JsonElement rootElement, string propertyName, int fallbackValue)
        {
            if (rootElement.TryGetProperty(propertyName, out JsonElement jsonValue))
            {
                if (jsonValue.ValueKind == JsonValueKind.Number && jsonValue.TryGetInt32(out Int32 parsedInteger))
                {
                    return parsedInteger;
                }

                if (jsonValue.ValueKind == JsonValueKind.String &&
                    int.TryParse(jsonValue.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out Int32 parsedFromString))
                {
                    return parsedFromString;
                }
            }

            return fallbackValue;
        }
    }
}

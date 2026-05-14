using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.TrailerScraping
{
    /// <summary>
    /// Orchestrates the full scrape flow: create job, search YouTube, download MP4, create Reels.
    /// Owner: Andrei.
    /// </summary>
    public class VideoIngestionService : IVideoIngestionService
    {
        private const int MaxTrailerDurationSeconds = 60;
        private const int DefaultCreatorUserId = 1;
        private const int SingleMovieScrapeCount = 1;

        private const string DefaultTrailerTitle = "Scraped Trailer";
        private const string SourceScraped = "scraped";
        private const string JobStatusRunning = "running";
        private const string JobStatusCompleted = "completed";
        private const string JobStatusFailed = "failed";
        private const string LogLevelInfo = "Info";
        private const string LogLevelWarn = "Warn";
        private const string LogLevelError = "Error";
        private const string UnknownErrorMessage = "unknown error";
        private const string TrailerSearchQuerySuffix = " official trailer";
        private const string FormatMp4 = "MP4";
        private const string FormatYouTubeUrl = "YouTube URL";

        private const string CaptionFormat = "Trailer for \"{0}\" — {1} | {2}";
        private const string LogFormatScrapingMovie = "Scraping trailers for movie: \"{0}\" (ID {1})";
        private const string LogFormatYouTubeQuery = "YouTube query: \"{0}\" (max {1} results)";
        private const string LogFormatYouTubeReturned = "YouTube returned {0} result(s)";
        private const string LogFormatReelExists = "Reel already exists for: {0}";
        private const string LogFormatDownloadingMp4 = "Downloading MP4: \"{0}\"...";
        private const string LogFormatMp4Failed = "MP4 download failed: {0}";
        private const string LogFormatSkippingNoMp4 = "Skipping \"{0}\" — no playable MP4 available.";
        private const string LogFormatCreatedReel = "Created reel (ID {0}) for \"{1}\" [{2}]";
        private const string LogFormatFailedToProcess = "Failed to process \"{0}\": {1}";
        private const string LogFormatJobCompleted = "Job completed — {0} reel(s) created for \"{1}\"";
        private const string LogFormatJobFailed = "Job failed: {0}";

        private readonly IYouTubeScraperService scraper;
        private readonly IScrapeJobRepository repository;
        private readonly IVideoDownloadService downloader;
        private readonly IServiceScopeFactory scopeFactory;

        public VideoIngestionService(
            IYouTubeScraperService scraper,
            IScrapeJobRepository repository,
            IVideoDownloadService downloader,
            IServiceScopeFactory scopeFactory)
        {
            this.scraper = scraper;
            this.repository = repository;
            this.downloader = downloader;
            this.scopeFactory = scopeFactory;
        }

        public async Task<string> IngestVideoFromUrlAsync(string trailerUrl, int movieId)
        {
            bool exists = await this.repository.ReelExistsByVideoUrlAsync(trailerUrl);
            if (exists) return string.Empty;

            string? localPath = await this.downloader.DownloadVideoAsMp4Async(trailerUrl, MaxTrailerDurationSeconds);
            if (string.IsNullOrEmpty(localPath)) return string.Empty;

            Movie? trackedMovie = await this.repository.GetMovieByIdAsync(movieId);
            User? trackedUser = await this.repository.GetUserByIdAsync(DefaultCreatorUserId);

            if (trackedMovie == null || trackedUser == null)
            {
                return string.Empty;
            }

            Reel reel = new Reel
            {
                Movie = trackedMovie,
                CreatorUser = trackedUser,
                VideoUrl = localPath,
                Title = DefaultTrailerTitle,
                Caption = string.Empty,
                ThumbnailUrl = string.Empty,
                Source = SourceScraped,
                CreatedAt = DateTime.UtcNow,
            };

            int reelId = await this.repository.InsertScrapedReelAsync(reel);
            return reelId.ToString();
        }

        public async Task<ScrapeJob> RunScrapeJobAsync(Movie movie, int maxResults, Func<ScrapeJobLog, Task>? onLogEntry = null)
        {
            string searchQuery = string.Concat(movie.Title, TrailerSearchQuerySuffix);

            ScrapeJob job = new ScrapeJob
            {
                SearchQuery = searchQuery,
                MaxResults = maxResults,
                Status = "pending",
                StartedAt = DateTime.UtcNow,
            };
            job.Id = await this.repository.CreateJobAsync(job);

            _ = Task.Run(async () =>
            {
                using IServiceScope scope = this.scopeFactory.CreateScope();
                var backgroundRepo = scope.ServiceProvider.GetRequiredService<IScrapeJobRepository>();
                var backgroundScraper = scope.ServiceProvider.GetRequiredService<IYouTubeScraperService>();
                var backgroundDownloader = scope.ServiceProvider.GetRequiredService<IVideoDownloadService>();

                var backgroundJob = await backgroundRepo.GetJobByIdAsync(job.Id);
                if (backgroundJob == null) return;

                Movie? trackedMovie = await backgroundRepo.GetMovieByIdAsync(movie.Id);
                User? trackedUser = await backgroundRepo.GetUserByIdAsync(DefaultCreatorUserId);

                async Task LogAsync(string level, string message)
                {
                    ScrapeJobLog logEntry = new ScrapeJobLog
                    {
                        ScrapeJob = backgroundJob,
                        Level = level,
                        Message = message,
                        Timestamp = DateTime.UtcNow,
                    };
                    await backgroundRepo.AddLogEntryAsync(logEntry);
                    if (onLogEntry is not null) await onLogEntry(logEntry);
                }

                try
                {
                    backgroundJob.Status = JobStatusRunning;
                    await backgroundRepo.UpdateJobAsync(backgroundJob);

                    await LogAsync(LogLevelInfo, string.Format(LogFormatScrapingMovie, movie.Title, movie.Id));
                    await LogAsync(LogLevelInfo, string.Format(LogFormatYouTubeQuery, searchQuery, maxResults));

                    IList<ScrapedVideoResult> results = await backgroundScraper.SearchVideosAsync(searchQuery, maxResults);
                    await LogAsync(LogLevelInfo, string.Format(LogFormatYouTubeReturned, results.Count));

                    int reelsCreated = 0;

                    foreach (ScrapedVideoResult video in results)
                    {
                        try
                        {
                            bool reelExists = await backgroundRepo.ReelExistsByVideoUrlAsync(video.VideoUrl);
                            if (reelExists)
                            {
                                await LogAsync(LogLevelWarn, string.Format(LogFormatReelExists, video.VideoUrl));
                                continue;
                            }

                            await LogAsync(LogLevelInfo, string.Format(LogFormatDownloadingMp4, video.Title));
                            string? localMp4Path = await backgroundDownloader.DownloadVideoAsMp4Async(video.VideoUrl, MaxTrailerDurationSeconds);

                            if (string.IsNullOrEmpty(localMp4Path))
                            {
                                string reason = backgroundDownloader.LastError ?? UnknownErrorMessage;
                                await LogAsync(LogLevelError, string.Format(LogFormatMp4Failed, reason));
                                await LogAsync(LogLevelWarn, string.Format(LogFormatSkippingNoMp4, video.Title));
                                continue;
                            }

                            Reel reel = new Reel
                            {
                                Movie = trackedMovie!,
                                CreatorUser = trackedUser!,
                                VideoUrl = localMp4Path,
                                ThumbnailUrl = video.ThumbnailUrl,
                                Title = video.Title,
                                Caption = string.Format(CaptionFormat, movie.Title, video.ChannelTitle, video.VideoUrl),
                                Source = SourceScraped,
                                CreatedAt = DateTime.UtcNow,
                            };

                            int reelId = await backgroundRepo.InsertScrapedReelAsync(reel);
                            reelsCreated++;

                            string format = !string.IsNullOrEmpty(localMp4Path) ? FormatMp4 : FormatYouTubeUrl;
                            await LogAsync(LogLevelInfo, string.Format(LogFormatCreatedReel, reelId, movie.Title, format));
                        }
                        catch (Exception exception)
                        {
                            await LogAsync(LogLevelError, string.Format(LogFormatFailedToProcess, video.Title, exception.Message));
                        }
                    }

                    backgroundJob.MoviesFound = SingleMovieScrapeCount;
                    backgroundJob.ReelsCreated = reelsCreated;
                    backgroundJob.Status = JobStatusCompleted;
                    backgroundJob.CompletedAt = DateTime.UtcNow;
                    await backgroundRepo.UpdateJobAsync(backgroundJob);

                    await LogAsync(LogLevelInfo, string.Format(LogFormatJobCompleted, reelsCreated, movie.Title));
                }
                catch (Exception exception)
                {
                    backgroundJob.Status = JobStatusFailed;
                    backgroundJob.CompletedAt = DateTime.UtcNow;
                    backgroundJob.ErrorMessage = exception.Message;
                    await backgroundRepo.UpdateJobAsync(backgroundJob);

                    try { await LogAsync(LogLevelError, string.Format(LogFormatJobFailed, exception.Message)); }
                    catch { }
                }
            });

            return job;
        }

        public async Task<ScrapeJob?> GetJobStatusAsync(int jobId)
        {
            return await this.repository.GetJobByIdAsync(jobId);
        }

        public async Task<IList<ScrapeJob>> GetAllJobsAsync()
        {
            return await this.repository.GetAllJobsAsync();
        }

        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            return await this.repository.GetAllReelsAsync();
        }
    }
}

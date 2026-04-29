namespace MovieApp.Features.TrailerScraping.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MovieApp.DataLayer.Models;

    /// <summary>
    /// Orchestrates the full scrape flow:
    /// 1. Create a ScrapeJob
    /// 2. Search YouTube for trailers of a selected movie
    /// 3. For each result: download MP4, create Reel (skip duplicates)
    /// 4. Update job status to completed/failed
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
        private readonly IScrapeRepository repository;
        private readonly IVideoDownloadService downloader;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoIngestionService"/> class.
        /// </summary>
        /// <param name="scraper">The YouTube scraper service.</param>
        /// <param name="repository">The scrape job repository.</param>
        /// <param name="downloader">The video download service.</param>
        public VideoIngestionService(
            IYouTubeScraperService scraper,
            IScrapeRepository repository,
            IVideoDownloadService downloader)
        {
            this.scraper = scraper;
            this.repository = repository;
            this.downloader = downloader;
        }

        /// <summary>
        /// Ingests a single video from a URL and associates it with a movie.
        /// </summary>
        /// <param name="trailerUrl">The URL of the trailer.</param>
        /// <param name="movieId">The ID of the movie.</param>
        /// <returns>The ID of the newly created reel, or an empty string if it fails or duplicates.</returns>
        public async Task<string> IngestVideoFromUrlAsync(string trailerUrl, int movieId)
        {
            bool exists = await this.repository.ReelExistsByVideoUrlAsync(trailerUrl);
            if (exists)
            {
                return string.Empty; // duplicate
            }

            // Download as MP4 — YouTube URLs are not directly playable
            string? localPath = await this.downloader.DownloadVideoAsMp4Async(trailerUrl, MaxTrailerDurationSeconds);

            if (string.IsNullOrEmpty(localPath))
            {
                return string.Empty; // download failed, skip
            }

            Reel reel = new Reel
            {
                Movie = new Movie { Id = movieId },
                CreatorUser = new User { Id = DefaultCreatorUserId },
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

        /// <summary>
        /// Runs a full scrape job for a specific movie from the Movie table.
        /// Searches YouTube for trailers, downloads them as MP4, and inserts Reels linked to the given MovieId.
        /// </summary>
        /// <param name="movie">The movie selected from the Movie table.</param>
        /// <param name="maxResults">Maximum number of YouTube results to fetch.</param>
        /// <param name="onLogEntry">Optional callback for live UI log updates.</param>
        /// <returns>The completed <see cref="ScrapeJob"/>.</returns>
        public async Task<ScrapeJob> RunScrapeJobAsync(
            Movie movie,
            int maxResults,
            Func<ScrapeJobLog, Task>? onLogEntry = null)
        {
            // Build search query from the movie title
            string searchQuery = string.Concat(movie.Title, TrailerSearchQuerySuffix);

            // 1. Create job record
            ScrapeJob job = new ScrapeJob
            {
                SearchQuery = searchQuery,
                MaxResults = maxResults,
                Status = JobStatusRunning,
                StartedAt = DateTime.UtcNow,
            };
            job.Id = await this.repository.CreateJobAsync(job);

            async Task LogAsync(string level, string message)
            {
                ScrapeJobLog logEntry = new ScrapeJobLog
                {
                    ScrapeJob = job,
                    Level = level,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                };
                await this.repository.AddLogEntryAsync(logEntry);
                if (onLogEntry is not null)
                {
                    await onLogEntry(logEntry);
                }
            }

            try
            {
                await LogAsync(LogLevelInfo, string.Format(LogFormatScrapingMovie, movie.Title, movie.Id));
                await LogAsync(LogLevelInfo, string.Format(LogFormatYouTubeQuery, searchQuery, maxResults));

                // 2. Search YouTube
                IList<ScrapedVideoResult> results = await this.scraper.SearchVideosAsync(searchQuery, maxResults);
                await LogAsync(LogLevelInfo, string.Format(LogFormatYouTubeReturned, results.Count));

                int reelsCreated = 0;

                // 3. Process each result
                foreach (ScrapedVideoResult video in results)
                {
                    try
                    {
                        // Check for duplicate reel
                        bool reelExists = await this.repository.ReelExistsByVideoUrlAsync(video.VideoUrl);
                        if (reelExists)
                        {
                            await LogAsync(LogLevelWarn, string.Format(LogFormatReelExists, video.VideoUrl));
                            continue;
                        }

                        // Download video as MP4 (max 60 seconds)
                        await LogAsync(LogLevelInfo, string.Format(LogFormatDownloadingMp4, video.Title));
                        string? localMp4Path = await this.downloader.DownloadVideoAsMp4Async(video.VideoUrl, MaxTrailerDurationSeconds);

                        // Skip this video if download failed
                        if (string.IsNullOrEmpty(localMp4Path))
                        {
                            string reason = this.downloader.LastError ?? UnknownErrorMessage;
                            await LogAsync(LogLevelError, string.Format(LogFormatMp4Failed, reason));
                            await LogAsync(LogLevelWarn, string.Format(LogFormatSkippingNoMp4, video.Title));
                            continue;
                        }

                        string videoUrl = localMp4Path;

                        // Insert Reel linked to the selected movie
                        Reel reel = new Reel
                        {
                            Movie = movie, 
                            CreatorUser = new User { Id = DefaultCreatorUserId },
                            VideoUrl = videoUrl,
                            ThumbnailUrl = video.ThumbnailUrl,
                            Title = video.Title,
                            Caption = string.Format(CaptionFormat, movie.Title, video.ChannelTitle, video.VideoUrl),
                            Source = SourceScraped,
                            CreatedAt = DateTime.UtcNow,
                        };

                        int reelId = await this.repository.InsertScrapedReelAsync(reel);
                        reelsCreated++;

                        string format = !string.IsNullOrEmpty(localMp4Path) ? FormatMp4 : FormatYouTubeUrl;
                        await LogAsync(LogLevelInfo, string.Format(LogFormatCreatedReel, reelId, movie.Title, format));
                    }
                    catch (Exception exception)
                    {
                        await LogAsync(LogLevelError, string.Format(LogFormatFailedToProcess, video.Title, exception.Message));
                    }
                }

                // 4. Complete job
                job.MoviesFound = SingleMovieScrapeCount; // We scraped for a single movie
                job.ReelsCreated = reelsCreated;
                job.Status = JobStatusCompleted;
                job.CompletedAt = DateTime.UtcNow;
                await this.repository.UpdateJobAsync(job);

                await LogAsync(LogLevelInfo, string.Format(LogFormatJobCompleted, reelsCreated, movie.Title));
            }
            catch (Exception exception)
            {
                job.Status = JobStatusFailed;
                job.CompletedAt = DateTime.UtcNow;
                job.ErrorMessage = exception.Message;
                await this.repository.UpdateJobAsync(job);

                try
                {
                    await LogAsync(LogLevelError, string.Format(LogFormatJobFailed, exception.Message));
                }
                catch
                {
                    /* best effort logging */
                }
            }

            return job;
        }
    }
}

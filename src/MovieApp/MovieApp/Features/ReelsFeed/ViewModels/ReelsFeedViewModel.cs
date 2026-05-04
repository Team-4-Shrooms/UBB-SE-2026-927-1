using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MovieApp;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.ReelsFeed;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace MovieApp.Features.ReelsFeed.ViewModels
{
    /// <summary>
    /// ViewModel for the Reels Feed page.
    /// Owner: Tudor.
    /// </summary>
    public partial class ReelsFeedViewModel : ObservableObject
    {
        private const int MockUserId = 1;
        private const int RecommendedReelCount = 10;
        private const int PrefetchRange = 2;
        private const int InitialQueueIndex = 0;
        private const double MinTrackedWatchSeconds = 0.5;
        private const double MaxWatchRatio = 1.0;
        private const double PercentageMultiplier = 100.0;

        private readonly IRecommendationService recommendationService;
        private readonly IClipPlaybackService clipPlaybackService;
        private readonly IReelInteractionService reelInteractionService;
        private readonly Dictionary<string, MediaPlaybackItem> prefetchedPlaybackItems =
            new Dictionary<string, MediaPlaybackItem>(StringComparer.OrdinalIgnoreCase);
        private readonly object prefetchLock = new ();

        /// <summary>Tracks wall-clock watch time for the currently visible reel.</summary>
        private readonly Stopwatch watchStopwatch = new ();
        private Reel? previousReel;

        [ObservableProperty]
        private string pageTitle = "Reels Feed";

        [ObservableProperty]
        private string statusMessage = "Loading...";

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? errorMessage;

        /// <summary>Gets a value indicating whether <see cref="ErrorMessage"/> has a value.</summary>
        public bool HasError => !string.IsNullOrEmpty(this.ErrorMessage);

        /// <summary>Visibility helper: true when feed loaded successfully but returned zero clips.</summary>
        [ObservableProperty]
        private bool isEmpty;

        [ObservableProperty]
        private Reel? currentReel;

        partial void OnErrorMessageChanged(string? value) => this.OnPropertyChanged(nameof(HasError));

        /// <summary>
        /// Gets the queue of reels currently loaded for the feed.
        /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<Reel> ReelQueue { get; } = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReelsFeedViewModel"/> class.
        /// </summary>
        /// <param name="recommendationService">Service used to retrieve recommended reels.</param>
        /// <param name="clipPlaybackService">Service used to prefetch and control clip playback state.</param>
        /// <param name="reelInteractionService">Service used to record reel views and likes.</param>
        public ReelsFeedViewModel(
            IRecommendationService recommendationService,
            IClipPlaybackService clipPlaybackService,
            IReelInteractionService reelInteractionService)
        {
            this.recommendationService = recommendationService;
            this.clipPlaybackService = clipPlaybackService;
            this.reelInteractionService = reelInteractionService;
        }

        /// <summary>
        /// Loads the reels feed, initializes current reel state, and updates UI status flags.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task LoadFeedAsync()
        {
            this.IsLoading = true;
            this.ErrorMessage = null;
            this.IsEmpty = false;
            this.ReelQueue.Clear();
            lock (this.prefetchLock)
            {
                this.prefetchedPlaybackItems.Clear();
            }

            try
            {
                var recommendedReels = await this.recommendationService.GetRecommendedReelsAsync(MockUserId, RecommendedReelCount);
                foreach (var recommendedReel in recommendedReels)
                {
                    this.ReelQueue.Add(recommendedReel);
                }

                // Load IsLiked and LikeCount onto each Reel so the UI can bind directly
                await this.LoadLikeDataAsync(recommendedReels);

                if (this.ReelQueue.Count > 0)
                {
                    this.CurrentReel = this.ReelQueue.First();
                    this.previousReel = this.CurrentReel;
                    this.watchStopwatch.Restart();
                    this.StatusMessage = string.Empty;
                    this.PrefetchNearby(InitialQueueIndex);
                }
                else
                {
                    this.IsEmpty = true;
                    this.StatusMessage = string.Empty;
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = $"Failed to load feed: {ex.Message}";
                this.StatusMessage = string.Empty;
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        private void PrefetchNearby(int currentIndex)
        {
            // 2 forward, 2 backward (excluding current)
            for (int offset = -PrefetchRange; offset <= PrefetchRange; offset++)
            {
                if (offset == InitialQueueIndex)
                {
                    continue;
                }

                int queueIndex = currentIndex + offset;
                if (queueIndex >= 0 && queueIndex < this.ReelQueue.Count)
                {
                    var nearbyReel = this.ReelQueue[queueIndex];
                    if (!string.IsNullOrEmpty(nearbyReel.VideoUrl))
                    {
                        _ = this.PrefetchPlaybackItemAsync(nearbyReel.VideoUrl);
                    }
                }
            }
        }

        /// <summary>
        /// Prefetches transmission metadata and eagerly builds a playback item cache entry.
        /// </summary>
        /// <param name="videoUrl">The reel video URL.</param>
        private async Task PrefetchPlaybackItemAsync(string videoUrl)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                return;
            }

            lock (this.prefetchLock)
            {
                if (this.prefetchedPlaybackItems.ContainsKey(videoUrl))
                {
                    return;
                }
            }

            await this.clipPlaybackService.PrefetchClipAsync(videoUrl);

            try
            {
                var transmission = this.clipPlaybackService.GetClipTransmission(videoUrl);
                var mediaSource = MediaSource.CreateFromUri(new Uri(transmission.VideoUrl));
                var playbackItem = new MediaPlaybackItem(mediaSource);

                lock (this.prefetchLock)
                {
                    if (!this.prefetchedPlaybackItems.ContainsKey(videoUrl))
                    {
                        this.prefetchedPlaybackItems[videoUrl] = playbackItem;
                    }
                }
            }
            catch
            {
            }
        }

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        /// <summary>
        /// Handles forward scroll selection changes and prefetches nearby reels.
        /// </summary>
        /// <param name="newCurrent">The newly selected reel.</param>
        public void ScrollNext(Reel newCurrent)
        {
            this.FlushWatchData();
            this.CurrentReel = newCurrent;
            this.previousReel = newCurrent;
            this.watchStopwatch.Restart();

            var queueIndex = this.ReelQueue.IndexOf(newCurrent);
            if (queueIndex >= 0)
            {
                this.PrefetchNearby(queueIndex);
            }
        }

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        /// <summary>
        /// Handles backward scroll selection changes and prefetches nearby reels.
        /// </summary>
        /// <param name="newCurrent">The newly selected reel.</param>
        public void ScrollPrevious(Reel newCurrent)
        {
            this.FlushWatchData();
            this.CurrentReel = newCurrent;
            this.previousReel = newCurrent;
            this.watchStopwatch.Restart();

            var queueIndex = this.ReelQueue.IndexOf(newCurrent);
            if (queueIndex >= 0)
            {
                this.PrefetchNearby(queueIndex);
            }
        }

        /// <summary>
        /// Persists watch-duration data for the reel the user just scrolled away from.
        /// Per Task 10: calls RecordViewAsync with elapsed seconds and watch percentage.
        /// </summary>
        private void FlushWatchData()
        {
            this.watchStopwatch.Stop();
            var previouslyVisibleReel = this.previousReel;
            if (previouslyVisibleReel == null)
            {
                return;
            }

            double watchedSeconds = this.watchStopwatch.Elapsed.TotalSeconds;
            if (watchedSeconds < MinTrackedWatchSeconds)
            {
                return; // ignore trivial flicks
            }

            double watchPercentage = this.CalculateWatchPercentage(watchedSeconds, (double)previouslyVisibleReel.FeatureDurationSeconds);

            // Fire-and-forget — feed stays navigable even if persistence fails (Task 10)
            _ = this.RecordFlushedWatchDataAsync(previouslyVisibleReel.Id, watchedSeconds, watchPercentage);
        }

        /// <summary>
        /// Persists flushed watch data without blocking the UI flow.
        /// </summary>
        private async Task RecordFlushedWatchDataAsync(int reelId, double watchedSeconds, double watchPercentage)
        {
            try
            {
                await this.reelInteractionService.RecordViewAsync(MockUserId, reelId, watchedSeconds, watchPercentage);
            }
            catch
            {
                // Logged server-side; feed continues.
            }
        }

        /// <summary>
        /// Called when the page unloads (e.g. window closing) to flush the final reel's watch data.
        /// </summary>
        public void OnNavigatingAway()
        {
            this.FlushWatchData();
        }

        /// <summary>
        /// Calculates watched percentage from elapsed seconds and reel duration.
        /// </summary>
        /// <param name="watchedSeconds">Observed watch duration in seconds.</param>
        /// <param name="featureDurationSeconds">Total reel duration in seconds.</param>
        /// <returns>Watch percentage in the range 0-100.</returns>
        private double CalculateWatchPercentage(double watchedSeconds, double featureDurationSeconds)
        {
            if (featureDurationSeconds <= 0)
            {
                return 0;
            }

            return Math.Min(watchedSeconds / featureDurationSeconds, MaxWatchRatio) * PercentageMultiplier;
        }

        /// <summary>
        /// Populates IsLiked and LikeCount on each Reel in the batch.
        /// </summary>
        private async Task LoadLikeDataAsync(IList<Reel> reelBatch)
        {
            foreach (var reelItem in reelBatch)
            {
                try
                {
                    var userInteraction = await this.reelInteractionService.GetInteractionAsync(MockUserId, reelItem.Id);
                    reelItem.IsLiked = userInteraction?.IsLiked ?? false;
                    reelItem.LikeCount = await this.reelInteractionService.GetLikeCountAsync(reelItem.Id);
                }
                catch
                {
                    reelItem.IsLiked = false;
                    reelItem.LikeCount = 0;
                }
            }
        }

        /// <summary>
        /// Builds a playback item for a reel using transmission data from the playback service.
        /// MediaSource creation is intentionally performed in ViewModel layer.
        /// </summary>
        /// <param name="videoUrl">The reel video URL.</param>
        /// <returns>A playback item for the player, or null if URL is invalid.</returns>
        public MediaPlaybackItem? BuildPlaybackItem(string? videoUrl)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                return null;
            }

            lock (this.prefetchLock)
            {
                if (this.prefetchedPlaybackItems.Remove(videoUrl, out var prefetchedPlaybackItem))
                {
                    return prefetchedPlaybackItem;
                }
            }

            try
            {
                var transmission = this.clipPlaybackService.GetClipTransmission(videoUrl);
                var mediaSource = MediaSource.CreateFromUri(new Uri(transmission.VideoUrl));
                return new MediaPlaybackItem(mediaSource);
            }
            catch
            {
                return null;
            }
        }
    }
}

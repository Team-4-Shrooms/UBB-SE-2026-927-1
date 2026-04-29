using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using MovieApp.DataLayer.Models;
using MovieApp.Features.ReelsFeed.Services;
using Windows.Media.Playback;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Dispatching;

namespace MovieApp.Features.ReelsFeed.Views
{
    /// <summary>
    /// Single reel card with video player, like button, genre badge, progress bar,
    /// double-tap gesture, and heart animations.
    /// Owner: Tudor.
    /// </summary>
    public sealed partial class ReelItemView : UserControl
    {
        private const int MockUserId = 1;
        private const int NoItemsCount = 0;
        private const int LastItemOffset = 1;
        private const int ProgressTimerIntervalMilliseconds = 250;
        private const double PercentageMultiplier = 100.0;
        private const double MaxProgressPercentage = 100.0;
        private const double DefaultScale = 1.0;
        private const double HeartBouncePeakScale = 1.4;
        private const int HeartBounceMidpointMilliseconds = 150;
        private const int HeartBounceDurationMilliseconds = 300;
        private const double HeartBurstInitialOpacity = 0.0;
        private const int HeartBurstFadeInMilliseconds = 100;
        private const int HeartBurstFadeOutMilliseconds = 600;
        private const double HeartBurstInitialScale = 0.5;
        private const int HeartBurstScaleUpMilliseconds = 200;
        private const double HeartBurstMidScale = 1.3;
        private const double HeartBurstFinalScale = 1.5;

        /// <summary>
        /// Gets or sets a value indicating whether the app is closing.
        /// Prevents any queued DispatcherQueue callbacks from touching XAML elements
        /// after the window starts tearing down.
        /// </summary>
        internal static bool IsAppClosing { get; set; }

        // Heart glyphs in Segoe MDL2 Assets
        private const string HeartOutline = "\uEB51";
        private const string HeartFilled = "\uEB52";

        private static readonly SolidColorBrush WhiteBrush = new (Colors.White);
        private static readonly SolidColorBrush RedBrush = new (Colors.Red);

        /// <summary>
        /// Identifies the <see cref="Reel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReelProperty =
            DependencyProperty.Register("Reel", typeof(Reel), typeof(ReelItemView), new PropertyMetadata(null, OnReelChanged));

        /// <summary>
        /// Gets or sets the reel displayed by this view.
        /// </summary>
        public Reel Reel
        {
            get
            {
                return (Reel)this.GetValue(ReelProperty);
            }

            set
            {
                this.SetValue(ReelProperty, value);
            }
        }

        private readonly IReelInteractionService interactionService;
        private DispatcherTimer? progressTimer;
        private string? loadedVideoUrl;

        /// <summary>
        /// Per-instance flag to prevent double-disposal and post-dispose access.
        /// Reset when the container is recycled via OnReelChanged.
        /// </summary>
        private volatile bool disposed;

        /// <summary>
        /// Stored reference so we can unhook PropertyChanged when the Reel changes
        /// or the control is recycled by the VirtualizingStackPanel.
        /// </summary>
        private Reel? subscribedReel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReelItemView"/> class.
        /// </summary>
        public ReelItemView()
        {
            this.InitializeComponent();
            this.interactionService = Ioc.Default.GetRequiredService<IReelInteractionService>();

            // Do NOT hook MediaEnded here — it's done in OnReelChanged after setting Source,
            // so we always hook the correct auto-created MediaPlayer instance.
            this.Unloaded += this.ReelItemView_Unloaded;
        }

        /// <summary>
        /// Releases resources when the control unloads.
        /// </summary>
        private void ReelItemView_Unloaded(object sender, RoutedEventArgs e)
        {
            this.StopProgressTimer();
            this.UnsubscribeFromReel();
            this.DisposeMediaPlayer();
        }

        /// <summary>
        /// Updates the view when the bound reel changes.
        /// </summary>
        /// <param name="d">The dependency object that owns the property.</param>
        /// <param name="e">The property change data.</param>
        private static void OnReelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ReelItemView view && e.NewValue is Reel reel)
            {
                // Unhook from the previous model before subscribing to the new one
                view.UnsubscribeFromReel();

                // Reset progress bar immediately so stale position doesn't linger
                view.PlaybackProgress.Value = 0;
                view.StopProgressTimer();

                // Dispose the PREVIOUS MediaPlayer before setting a new source.
                // This prevents orphaned COM objects from container recycling.
                view.DisposeCurrentPlayer();

                // Container is being reused — clear the disposed flag
                view.disposed = false;

                // Set the new source — MediaPlayerElement will auto-create a new MediaPlayer
                view.ReelPlayer.Source = null;
                view.loadedVideoUrl = null;

                // Hook MediaEnded on the newly created MediaPlayer
                if (view.ReelPlayer.MediaPlayer != null)
                {
                    view.ReelPlayer.MediaPlayer.IsLoopingEnabled = false;
                    view.ReelPlayer.MediaPlayer.MediaEnded += view.MediaPlayer_MediaEnded;
                }

                // Sync like visuals from model state
                view.UpdateLikeVisuals(reel.IsLiked, reel.LikeCount);

                // Update genre badge
                view.UpdateGenreBadge(reel.Genre);

                // Listen for model property changes (e.g. if liked state loaded async after binding)
                view.subscribedReel = reel;
                reel.PropertyChanged += view.OnReelPropertyChanged;
            }
        }

        /// <summary>
        /// Reacts to reel model changes that affect the like visuals.
        /// </summary>
        /// <param name="sender">The reel model that raised the event.</param>
        /// <param name="args">The property change arguments.</param>
        private void OnReelPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (ReelItemView.IsAppClosing || this.disposed)
            {
                return;
            }

            if (args.PropertyName is nameof(Reel.IsLiked) or nameof(Reel.LikeCount))
            {
                if (sender is Reel reel)
                {
                    try
                    {
                        var dispatcherQueue = this.DispatcherQueue;
                        if (dispatcherQueue == null)
                        {
                            return;
                        }

                        void UpdateLikeVisualsOnDispatcher()
                        {
                            if (ReelItemView.IsAppClosing || this.disposed)
                            {
                                return;
                            }

                            try
                            {
                                //this.UpdateLikeVisuals(reel.IsLiked, reel.LikeCount);
                            }
                            catch
                            {
                                // view may be torn down
                            }
                        }

                        dispatcherQueue.TryEnqueue(UpdateLikeVisualsOnDispatcher);
                    }
                    catch
                    {
                        // DispatcherQueue torn down during close
                    }
                }
            }
        }

        /// <summary>
        /// Detaches the property-changed handler from the current reel model.
        /// </summary>
        private void UnsubscribeFromReel()
        {
            if (this.subscribedReel != null)
            {
                //this.subscribedReel.PropertyChanged -= this.OnReelPropertyChanged;
                this.subscribedReel = null;
            }
        }

        /// <summary>
        /// Updates the genre badge text and visibility.
        /// </summary>
        /// <param name="genre">The genre text to show, or <c>null</c> to hide the badge.</param>
        private void UpdateGenreBadge(string? genre)
        {
            if (!string.IsNullOrEmpty(genre))
            {
                this.GenreText.Text = genre;
                this.GenreBadge.Visibility = Visibility.Visible;
            }
            else
            {
                this.GenreBadge.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Synchronizes the heart icon and like count with the reel state.
        /// </summary>
        /// <param name="isLiked">A value indicating whether the reel is liked.</param>
        /// <param name="likeCount">The current like count.</param>
        private void UpdateLikeVisuals(bool isLiked, int likeCount)
        {
            this.HeartIcon.Glyph = isLiked ? HeartFilled : HeartOutline;
            this.HeartIcon.Foreground = isLiked ? RedBrush : WhiteBrush;
            this.LikeCountText.Text = likeCount.ToString();
        }

        /// <summary>
        /// Handles the like button click by toggling the current reel state.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The routed event data.</param>
        private async void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            await this.ToggleLikeWithAnimationAsync();
        }

        /// <summary>
        /// Double-tap gesture toggles like and plays heart burst animation at center.
        /// Per Task 12: double-tap gesture recognizer triggers ToggleLike + heart burst.
        /// </summary>
        private async void ReelItemView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (this.Reel == null)
            {
                return;
            }

            // Only like on double-tap (never unlike)
            if (!this.Reel.IsLiked)
            {
                await this.ToggleLikeWithAnimationAsync();
            }

            this.PlayHeartBurstAnimation();
        }

        /// <summary>
        /// Toggles the reel like state and plays the associated animation.
        /// </summary>
        /// <returns>A task that completes when the persistence call finishes.</returns>
        private async Task ToggleLikeWithAnimationAsync()
        {
            if (this.Reel == null)
            {
                return;
            }

            // Optimistic UI update
            bool wasLiked = this.Reel.IsLiked;
            this.Reel.IsLiked = !wasLiked;
            this.Reel.LikeCount += wasLiked ? -1 : 1;

            // Scale-bounce animation on heart icon (Task 12)
            this.PlayHeartBounceAnimation();

            try
            {
                await this.interactionService.ToggleLikeAsync(MockUserId, this.Reel.Id);
            }
            catch
            {
                // Revert on failure
                this.Reel.IsLiked = wasLiked;
                this.Reel.LikeCount += wasLiked ? 1 : -1;
            }
        }

        /// <summary>
        /// Plays the compact heart bounce animation on the like icon.
        /// </summary>
        private void PlayHeartBounceAnimation()
        {
            try
            {
                Storyboard storyboard = new Storyboard();

                DoubleAnimationUsingKeyFrames scaleX = new DoubleAnimationUsingKeyFrames();
                scaleX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = DefaultScale });
                scaleX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(HeartBounceMidpointMilliseconds), Value = HeartBouncePeakScale });
                scaleX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(HeartBounceDurationMilliseconds), Value = DefaultScale });
                Storyboard.SetTarget(scaleX, this.HeartScale);
                Storyboard.SetTargetProperty(scaleX, "ScaleX");

                DoubleAnimationUsingKeyFrames scaleY = new DoubleAnimationUsingKeyFrames();
                scaleY.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = DefaultScale });
                scaleY.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(HeartBounceMidpointMilliseconds), Value = HeartBouncePeakScale });
                scaleY.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(HeartBounceDurationMilliseconds), Value = DefaultScale });
                Storyboard.SetTarget(scaleY, this.HeartScale);
                Storyboard.SetTargetProperty(scaleY, "ScaleY");

                storyboard.Children.Add(scaleX);
                storyboard.Children.Add(scaleY);
                storyboard.Begin();
            }
            catch
            {
                // animation on torn-down element
            }
        }

        /// <summary>
        /// Plays the large heart burst animation used for double-tap likes.
        /// </summary>
        private void PlayHeartBurstAnimation()
        {
            try
            {
                Storyboard storyboard = new Storyboard();

                DoubleAnimationUsingKeyFrames opacity = new DoubleAnimationUsingKeyFrames();
                opacity.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = HeartBurstInitialOpacity });
                opacity.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(HeartBurstFadeInMilliseconds), Value = DefaultScale });
                opacity.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(HeartBurstFadeOutMilliseconds), Value = HeartBurstInitialOpacity });
                Storyboard.SetTarget(opacity, this.HeartBurst);
                Storyboard.SetTargetProperty(opacity, "Opacity");

                DoubleAnimationUsingKeyFrames burstScaleX = new DoubleAnimationUsingKeyFrames();
                burstScaleX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = HeartBurstInitialScale });
                burstScaleX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(HeartBurstScaleUpMilliseconds), Value = HeartBurstMidScale });
                burstScaleX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(HeartBurstFadeOutMilliseconds), Value = HeartBurstFinalScale });
                Storyboard.SetTarget(burstScaleX, this.BurstScale);
                Storyboard.SetTargetProperty(burstScaleX, "ScaleX");

                DoubleAnimationUsingKeyFrames burstScaleY = new DoubleAnimationUsingKeyFrames();
                burstScaleY.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = HeartBurstInitialScale });
                burstScaleY.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(HeartBurstScaleUpMilliseconds), Value = HeartBurstMidScale });
                burstScaleY.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(HeartBurstFadeOutMilliseconds), Value = HeartBurstFinalScale });
                Storyboard.SetTarget(burstScaleY, this.BurstScale);
                Storyboard.SetTargetProperty(burstScaleY, "ScaleY");

                storyboard.Children.Add(opacity);
                storyboard.Children.Add(burstScaleX);
                storyboard.Children.Add(burstScaleY);
                storyboard.Begin();
            }
            catch
            {
                // animation on torn-down element
            }
        }

        // ── Playback ──────────────────────────────────────────────────────

        /// <summary>
        /// Starts video playback for the current reel and begins progress updates.
        /// </summary>
        public void PlayVideo()
        {
            if (this.disposed || ReelItemView.IsAppClosing)
            {
                return;
            }

            try
            {
                if (this.ReelPlayer.MediaPlayer != null)
                {
                    this.ReelPlayer.MediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
                    this.ReelPlayer.MediaPlayer.Play();
                    this.StartProgressTimer();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Pauses video playback for the current reel and stops progress updates.
        /// </summary>
        public void PauseVideo()
        {
            if (this.disposed)
            {
                return;
            }

            try
            {
                if (this.ReelPlayer.MediaPlayer != null)
                {
                    this.ReelPlayer.MediaPlayer.Pause();
                    this.StopProgressTimer();
                }
            }
            catch
            {
            }
        }

        // ── Progress bar timer ────────────────────────────────────────────

        /// <summary>
        /// Starts the timer that keeps the playback progress bar in sync.
        /// </summary>
        private void StartProgressTimer()
        {
            if (this.progressTimer == null)
            {
                this.progressTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(ProgressTimerIntervalMilliseconds) };
                this.progressTimer.Tick += this.ProgressTimer_Tick;
            }

            this.progressTimer.Start();
        }

        /// <summary>
        /// Stops the playback progress timer and releases its tick handler.
        /// </summary>
        private void StopProgressTimer()
        {
            if (this.progressTimer != null)
            {
                this.progressTimer.Stop();
                this.progressTimer.Tick -= this.ProgressTimer_Tick;
                this.progressTimer = null;
            }
        }

        /// <summary>
        /// Updates the playback progress bar from the current media session.
        /// </summary>
        /// <param name="sender">The timer that raised the tick.</param>
        /// <param name="e">The event data.</param>
        private void ProgressTimer_Tick(object? sender, object e)
        {
            if (ReelItemView.IsAppClosing || this.disposed)
            {
                this.StopProgressTimer();
                return;
            }

            try
            {
                MediaPlayer player = this.ReelPlayer.MediaPlayer;
                if (player == null)
                {
                    this.StopProgressTimer();
                    return;
                }

                MediaPlaybackSession session = player.PlaybackSession;
                if (session.NaturalDuration.TotalSeconds > 0)
                {
                    double progressPercentage = (session.Position.TotalSeconds / session.NaturalDuration.TotalSeconds) * PercentageMultiplier;
                    this.PlaybackProgress.Value = Math.Min(progressPercentage, MaxProgressPercentage);
                }
            }
            catch
            {
                // COM object may be revoked during teardown — stop polling
                this.StopProgressTimer();
            }
        }

        // ── Disposal ──────────────────────────────────────────────────────

        /// <summary>
        /// Tears down the current MediaPlayer without setting <c>_disposed</c>,
        /// so the container can be recycled with a new Source in OnReelChanged.
        /// </summary>
        private void DisposeCurrentPlayer()
        {
            try
            {
                MediaPlayer player = this.ReelPlayer.MediaPlayer;
                if (player == null)
                {
                    return;
                }

                // 1. Unhook events first — prevents callbacks during teardown
                player.MediaEnded -= this.MediaPlayer_MediaEnded;

                // 2. Pause and detach sources safely while COM player is still active
                player.Pause();
                IDisposable previousSource = this.ReelPlayer.Source as IDisposable;

                // 3. Sever XAML element connection (stops internal MF callbacks)
                this.ReelPlayer.Source = null;
                this.ReelPlayer.SetMediaPlayer(null);

                // 4. Dispose safely
                player.Source = null;
                previousSource?.Dispose();
                player.Dispose();
                this.loadedVideoUrl = null;
            }
            catch
            {
                // Swallow — we are tearing down
            }
        }

        /// <summary>
        /// Final disposal — sets <c>_disposed</c> to block all future access.
        /// Called by Unloaded and by MainWindow.Closed tree walk.
        /// </summary>
        public void DisposeMediaPlayer()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;

            this.StopProgressTimer();
            this.DisposeCurrentPlayer();
        }

        /// <summary>
        /// Walks up the visual tree to find the containing <see cref="FlipView"/>.
        /// </summary>
        /// <param name="element">The starting element.</param>
        /// <returns>The nearest parent <see cref="FlipView"/>, or <c>null</c> if none exists.</returns>
        private FlipView? GetParentFlipView(DependencyObject element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                if (parent is FlipView flipView)
                {
                    return flipView;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        private void MediaPlayer_MediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            // This callback fires on a Media Foundation background thread.
            // Do NOT access any DependencyProperty here.
            if (ReelItemView.IsAppClosing || this.disposed)
            {
                return;
            }

            // Capture DispatcherQueue reference while we know it's valid.
            // TryEnqueue returns false if the queue is shut down — no exception.
            try
            {
                DispatcherQueue dispatcherQueue = this.DispatcherQueue; 
                if (dispatcherQueue == null)
                {
                    return;
                }

                void AdvanceToNextReelOnDispatcher()
                {
                    if (ReelItemView.IsAppClosing || this.disposed)
                    {
                        return;
                    }

                    try
                    {
                        MediaPlayer mediaPlayer = this.ReelPlayer?.MediaPlayer;
                        if (mediaPlayer == null)
                        {
                            return;
                        }

                        FlipView parentFlipView = this.GetParentFlipView(this);
                        if (parentFlipView != null && parentFlipView.SelectedIndex < parentFlipView.Items.Count - LastItemOffset)
                        {
                            parentFlipView.SelectedIndex++;
                        }
                    }
                    catch
                    {
                        // view may be torn down
                    }
                }

                dispatcherQueue.TryEnqueue(AdvanceToNextReelOnDispatcher);
            }
            catch
            {
                // DispatcherQueue may already be torn down — this is expected during close
            }
        }

        /// <summary>
        /// Sets the playback item to the internal player element.
        /// </summary>
        /// <param name="videoUrl">The reel URL represented by this playback item.</param>
        /// <param name="playbackItem">Playback item built by the ViewModel.</param>
        public void SetPlaybackItem(string? videoUrl, MediaPlaybackItem? playbackItem)
        {
            if (this.disposed || ReelItemView.IsAppClosing)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(videoUrl) &&
                string.Equals(this.loadedVideoUrl, videoUrl, StringComparison.OrdinalIgnoreCase) &&
                this.ReelPlayer.Source != null)
            {
                return;
            }

            try
            {
                this.DisposeCurrentPlayer();
                this.disposed = false;
                this.ReelPlayer.Source = playbackItem;
                this.loadedVideoUrl = videoUrl;

                if (this.ReelPlayer.MediaPlayer != null)
                {
                    this.ReelPlayer.MediaPlayer.IsLoopingEnabled = false;
                    this.ReelPlayer.MediaPlayer.MediaEnded += this.MediaPlayer_MediaEnded;
                }
            }
            catch
            {
            }
        }
    }
}

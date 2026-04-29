namespace MovieApp.Features.ReelsEditing.Views
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using MovieApp.Features.ReelsEditing.ViewModels;
    using Windows.Media.Core;
    using Windows.Media.Playback;
    using CommunityToolkit.Mvvm.DependencyInjection;
    using MovieApp.DataLayer.Models;

    /// <summary>
    /// The page responsible for the reels editing interface.
    /// </summary>
    public sealed partial class ReelsEditingPage : Page
    {
        private const int VideoProgressIntervalMilliseconds = 250;
        private const double SliderValueTolerance = 0.001;
        private const double PercentageDivisor = 100.0;
        private const double MinimumCropPadding = 20.0;
        private const int MusicDialogHeight = 300;

        private const string DefaultTimeDisplay = "0:00";
        private const string TimeFormatHours = @"h\:mm\:ss";
        private const string TimeFormatMinutes = @"m\:ss";
        private const string ErrorNoMusicSelected = "No music track selected. Choose a track first.";
        private const string ErrorInvalidMusicUrl = "Music URL is not valid.";
        private const string MessagePlayingPreviewFormat = "Playing preview from {0:mm\\:ss}…";
        private const string MessagePreviewFailedFormat = "Preview failed: {0}";
        private const string DialogTitleChooseMusic = "Choose Background Music";
        private const string DialogButtonCancel = "Cancel";
        private const string DialogButtonConfirm = "Confirm";
        private const string TrackItemDataTemplateXml =
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
            "<TextBlock Text=\"{Binding TrackName}\" Padding=\"8,4\"/>" +
            "</DataTemplate>";

        private readonly MediaPlayer musicPreviewPlayer = new ();
        private readonly DispatcherTimer videoProgressTimer = new ()
        {
            Interval = TimeSpan.FromMilliseconds(VideoProgressIntervalMilliseconds),
        };

        private MediaPlayer? subscribedVideoPlayer;
        private bool isSyncingVideoControls;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReelsEditingPage"/> class.
        /// </summary>
        public ReelsEditingPage()
        {
            this.ViewModel = Ioc.Default.GetRequiredService<ReelsEditingViewModel>();
            this.GalleryViewModel = Ioc.Default.GetRequiredService<ReelGalleryViewModel>();
            this.MusicDialogViewModel = Ioc.Default.GetRequiredService<MusicSelectionDialogViewModel>();
            this.InitializeComponent();

            this.videoProgressTimer.Tick += this.VideoProgressTimer_Tick;
            this.Unloaded += this.Page_Unloaded;
            this.ResetVideoTransportUi();

            this.ViewModel.CropModeEntered += this.OnCropModeEntered;
            this.ViewModel.CropModeExited += this.OnCropModeExited;
            this.ViewModel.CropSaveStarted += this.OnCropSaveStarted;
            this.ViewModel.CropVideoUpdated += this.OnCropVideoUpdated;

            this.ViewModel.PropertyChanged += async (sender, eventArguments) =>
            {
                if (eventArguments.PropertyName == nameof(this.ViewModel.IsEditing))
                {
                    this.UpdatePanelVisibility();

                    if (!this.ViewModel.IsEditing)
                    {
                        this.StopMusicPreview();
                        await this.GalleryViewModel.LoadReelsCommand.ExecuteAsync(null);
                    }
                }
            };
        }

        /// <summary>
        /// Gets the ViewModel for editing the reel.
        /// </summary>
        public ReelsEditingViewModel ViewModel { get; }

        /// <summary>
        /// Gets the ViewModel for the reel gallery.
        /// </summary>
        public ReelGalleryViewModel GalleryViewModel { get; }

        /// <summary>
        /// Gets the ViewModel for the music selection dialog.
        /// </summary>
        public MusicSelectionDialogViewModel MusicDialogViewModel { get; }

        /// <summary>
        /// Converts a boolean status flag to a visibility representation.
        /// </summary>
        /// <param name="hasStatus">True if there is a status message.</param>
        /// <returns>Visible if true, otherwise Collapsed.</returns>
        public Visibility GetStatusVisibility(bool hasStatus)
            => hasStatus ? Visibility.Visible : Visibility.Collapsed;

        private static string FormatPlaybackTime(TimeSpan time)
        {
            if (time.TotalHours >= 1)
            {
                return time.ToString(TimeFormatHours);
            }

            return time.ToString(TimeFormatMinutes);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs eventArguments)
        {
            await this.GalleryViewModel.EnsureLoadedAsync();
            if (!this.videoProgressTimer.IsEnabled)
            {
                this.videoProgressTimer.Start();
            }

            this.AttachVideoPlayerEvents();
            this.UpdateVideoTransportUi();
            this.UpdatePanelVisibility();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs eventArguments)
        {
            this.videoProgressTimer.Stop();
            this.DetachVideoPlayerEvents();
            this.StopMusicPreview();
        }

        private void UpdatePanelVisibility()
        {
            if (this.ViewModel.IsEditing)
            {
                this.GalleryPanel.Visibility = Visibility.Collapsed;
                this.EditorPanel.Visibility = Visibility.Visible;
            }
            else
            {
                this.StopVideo();
                this.GalleryPanel.Visibility = Visibility.Visible;
                this.EditorPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ReelGridView_SelectionChanged(object sender, SelectionChangedEventArgs eventArguments)
        {
            // Selection tracked by TwoWay binding
        }

        private async void ReelGridView_ItemClick(object sender, ItemClickEventArgs eventArguments)
        {
            if (eventArguments.ClickedItem is DataLayer.Models.Reel reel)
            {
                this.GalleryViewModel.SelectedReel = reel;
                await this.ViewModel.LoadReelAsync(reel);
                this.LoadVideo(reel.VideoUrl);
            }
        }

        private void LoadVideo(string videoUrl)
        {
            try
            {
                this.DetachVideoPlayerEvents();
                if (!string.IsNullOrEmpty(videoUrl) && Uri.TryCreate(videoUrl, UriKind.Absolute, out Uri uri))
                {
                    this.ReelPlayer.Source = MediaSource.CreateFromUri(uri);
                    this.AttachVideoPlayerEvents();
                }
            }
            catch
            {
                // Video URL may be invalid; player will show empty
            }

            this.UpdateVideoTransportUi();
        }

        private void StopVideo()
        {
            try
            {
                this.DetachVideoPlayerEvents();
                this.ReelPlayer.Source = null;
            }
            catch
            {
                // Ignore cleanup errors
            }

            this.ResetVideoTransportUi();
        }

        private void AttachVideoPlayerEvents()
        {
            MediaPlayer mediaPlayer = this.ReelPlayer.MediaPlayer;
            if (mediaPlayer == null || ReferenceEquals(mediaPlayer, this.subscribedVideoPlayer))
            {
                return;
            }

            this.DetachVideoPlayerEvents();
            this.subscribedVideoPlayer = mediaPlayer;
            this.subscribedVideoPlayer.MediaOpened += this.ReelPlayer_MediaOpened;
            this.subscribedVideoPlayer.MediaEnded += this.ReelPlayer_MediaEnded;
        }

        private void DetachVideoPlayerEvents()
        {
            if (this.subscribedVideoPlayer == null)
            {
                return;
            }

            this.subscribedVideoPlayer.MediaOpened -= this.ReelPlayer_MediaOpened;
            this.subscribedVideoPlayer.MediaEnded -= this.ReelPlayer_MediaEnded;
            this.subscribedVideoPlayer = null;
        }

        private void ReelPlayer_MediaOpened(MediaPlayer sender, object eventArguments)
            => this.DispatcherQueue.TryEnqueue(this.UpdateVideoTransportUi);

        private void ReelPlayer_MediaEnded(MediaPlayer sender, object eventArguments)
            => this.DispatcherQueue.TryEnqueue(this.UpdateVideoTransportUi);

        private void VideoProgressTimer_Tick(object? sender, object eventArguments)
            => this.UpdateVideoTransportUi();

        private void VideoPlayPauseButton_Click(object sender, RoutedEventArgs eventArguments)
        {
            MediaPlayer mediaPlayer = this.ReelPlayer.MediaPlayer;
            if (mediaPlayer == null)
            {
                return;
            }

            if (mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                mediaPlayer.Pause();
            }
            else
            {
                mediaPlayer.Play();
            }

            this.UpdateVideoTransportUi();
        }

        private void VideoSeekSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs eventArguments)
        {
            if (this.isSyncingVideoControls || Math.Abs(eventArguments.NewValue - eventArguments.OldValue) < SliderValueTolerance)
            {
                return;
            }

            MediaPlayer mediaPlayer = this.ReelPlayer.MediaPlayer;
            MediaPlaybackSession session = mediaPlayer?.PlaybackSession;
            if (session == null)
            {
                return;
            }

            Double durationSeconds = session.NaturalDuration.TotalSeconds;
            if (durationSeconds <= 0)
            {
                return;
            }

            Double clampedSeconds = Math.Clamp(eventArguments.NewValue, 0, durationSeconds);
            session.Position = TimeSpan.FromSeconds(clampedSeconds);
            this.UpdateVideoTransportUi();
        }

        private void VideoMuteButton_Checked(object sender, RoutedEventArgs eventArguments)
        {
            if (this.isSyncingVideoControls)
            {
                return;
            }

            if (this.ReelPlayer.MediaPlayer != null)
            {
                this.ReelPlayer.MediaPlayer.IsMuted = true;
            }

            this.UpdateVideoTransportUi();
        }

        private void VideoMuteButton_Unchecked(object sender, RoutedEventArgs eventArguments)
        {
            if (this.isSyncingVideoControls)
            {
                return;
            }

            if (this.ReelPlayer.MediaPlayer != null)
            {
                this.ReelPlayer.MediaPlayer.IsMuted = false;
            }

            this.UpdateVideoTransportUi();
        }

        private void VideoFullscreenButton_Checked(object sender, RoutedEventArgs eventArguments)
        {
            if (this.isSyncingVideoControls)
            {
                return;
            }

            this.ReelPlayer.IsFullWindow = true;
            this.UpdateVideoTransportUi();
        }

        private void VideoFullscreenButton_Unchecked(object sender, RoutedEventArgs eventArguments)
        {
            if (this.isSyncingVideoControls)
            {
                return;
            }

            this.ReelPlayer.IsFullWindow = false;
            this.UpdateVideoTransportUi();
        }

        private void UpdateVideoTransportUi()
        {
            MediaPlayer mediaPlayer = this.ReelPlayer.MediaPlayer;
            MediaPlaybackSession session = mediaPlayer?.PlaybackSession;

            if (mediaPlayer == null || session == null || this.ReelPlayer.Source == null)
            {
                this.ResetVideoTransportUi();
                return;
            }

            TimeSpan duration = session.NaturalDuration;
            Double durationSeconds = Math.Max(duration.TotalSeconds, 0);
            Double currentSeconds = Math.Max(session.Position.TotalSeconds, 0);
            Double clampedCurrent = durationSeconds > 0
                ? Math.Min(currentSeconds, durationSeconds)
                : 0;

            this.isSyncingVideoControls = true;
            try
            {
                this.VideoSeekSlider.Maximum = durationSeconds;
                this.VideoSeekSlider.Value = clampedCurrent;
                this.VideoCurrentTimeText.Text = FormatPlaybackTime(TimeSpan.FromSeconds(clampedCurrent));
                this.VideoDurationText.Text = FormatPlaybackTime(duration);
                this.VideoPlayPauseIcon.Symbol = session.PlaybackState == MediaPlaybackState.Playing
                    ? Symbol.Pause
                    : Symbol.Play;
                this.VideoMuteButton.IsChecked = mediaPlayer.IsMuted;
                this.VideoMuteIcon.Symbol = mediaPlayer.IsMuted ? Symbol.Mute : Symbol.Volume;
                this.VideoFullscreenButton.IsChecked = this.ReelPlayer.IsFullWindow;
            }
            finally
            {
                this.isSyncingVideoControls = false;
            }
        }

        private void ResetVideoTransportUi()
        {
            this.isSyncingVideoControls = true;
            try
            {
                this.VideoSeekSlider.Maximum = 0;
                this.VideoSeekSlider.Value = 0;
                this.VideoCurrentTimeText.Text = DefaultTimeDisplay;
                this.VideoDurationText.Text = DefaultTimeDisplay;
                this.VideoPlayPauseIcon.Symbol = Symbol.Play;
                this.VideoMuteButton.IsChecked = false;
                this.VideoMuteIcon.Symbol = Symbol.Volume;
                this.VideoFullscreenButton.IsChecked = false;
            }
            finally
            {
                this.isSyncingVideoControls = false;
            }
        }

        private void OnCropModeEntered()
        {
            try
            {
                this.ReelPlayer.MediaPlayer?.Pause();
            }
            catch
            {
                // Ignore pause errors
            }

            this.CropOverlayRoot.Visibility = Visibility.Visible;
            this.UpdateCropOverlay();
        }

        private void OnCropModeExited()
        {
            this.CropOverlayRoot.Visibility = Visibility.Collapsed;
        }

        private void OnCropSaveStarted()
        {
            try
            {
                this.ReelPlayer.MediaPlayer?.Pause();
            }
            catch
            {
                // Ignore pause errors
            }

            this.StopVideo();
        }

        private void OnCropVideoUpdated(string videoUrl)
            => this.LoadVideo(videoUrl);

        private void CropResumePreview_Click(object sender, RoutedEventArgs eventArguments)
        {
            this.ReelPlayer.MediaPlayer?.Play();
            this.UpdateVideoTransportUi();
        }

        private void CropPausePreview_Click(object sender, RoutedEventArgs eventArguments)
        {
            this.ReelPlayer.MediaPlayer?.Pause();
            this.UpdateVideoTransportUi();
        }

        private void CropMarginSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs eventArguments)
            => this.UpdateCropOverlay();

        private void CropOverlayRoot_SizeChanged(object sender, SizeChangedEventArgs eventArguments)
            => this.UpdateCropOverlay();

        private void UpdateCropOverlay()
        {
            if (this.CropOverlayRoot.Visibility != Visibility.Visible)
            {
                return;
            }

            Double width = this.CropOverlayRoot.ActualWidth;
            Double height = this.CropOverlayRoot.ActualHeight;
            if (width <= 0 || height <= 0)
            {
                return;
            }

            Double left = width * (this.ViewModel.CropMarginLeft / PercentageDivisor);
            Double top = height * (this.ViewModel.CropMarginTop / PercentageDivisor);
            Double right = width * (this.ViewModel.CropMarginRight / PercentageDivisor);
            Double bottom = height * (this.ViewModel.CropMarginBottom / PercentageDivisor);

            if (left + right > width - MinimumCropPadding)
            {
                right = Math.Max(0, width - left - MinimumCropPadding);
            }

            if (top + bottom > height - MinimumCropPadding)
            {
                bottom = Math.Max(0, height - top - MinimumCropPadding);
            }

            this.CropRectangleBorder.Margin = new Thickness(left, top, right, bottom);
        }

        private void PlayMusicPreview_Click(object sender, RoutedEventArgs eventArguments)
        {
            MusicTrack track = this.ViewModel.SelectedMusicTrack;
            if (track == null || string.IsNullOrWhiteSpace(track.AudioUrl))
            {
                this.ViewModel.StatusMessage = ErrorNoMusicSelected;
                this.ViewModel.IsStatusSuccess = false;
                return;
            }

            try
            {
                if (!Uri.TryCreate(track.AudioUrl, UriKind.Absolute, out Uri uri))
                {
                    this.ViewModel.StatusMessage = ErrorInvalidMusicUrl;
                    this.ViewModel.IsStatusSuccess = false;
                    return;
                }

                this.musicPreviewPlayer.Source = MediaSource.CreateFromUri(uri);
                this.musicPreviewPlayer.Volume = this.ViewModel.MusicVolume / PercentageDivisor;

                TimeSpan startOffset = TimeSpan.FromSeconds(this.ViewModel.MusicStartTime);
                this.musicPreviewPlayer.MediaOpened += OnMusicMediaOpened;
                void OnMusicMediaOpened(MediaPlayer mediaPlayer, object? mediaEventArguments)
                {
                    mediaPlayer.MediaOpened -= OnMusicMediaOpened;
                    mediaPlayer.PlaybackSession.Position = startOffset;
                    mediaPlayer.Play();
                }

                this.ViewModel.StatusMessage = string.Format(MessagePlayingPreviewFormat, startOffset);
                this.ViewModel.IsStatusSuccess = true;
            }
            catch (Exception exception)
            {
                this.ViewModel.StatusMessage = string.Format(MessagePreviewFailedFormat, exception.Message);
                this.ViewModel.IsStatusSuccess = false;
            }
        }

        private void StopMusicPreview_Click(object sender, RoutedEventArgs eventArguments)
            => this.StopMusicPreview();

        private void StopMusicPreview()
        {
            try
            {
                this.musicPreviewPlayer.Pause();
                this.musicPreviewPlayer.Source = null;
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        private async void ChooseMusicButton_Click(object sender, RoutedEventArgs eventArguments)
        {
            await this.MusicDialogViewModel.LoadTracksCommand.ExecuteAsync(null);

            ContentDialog dialog = new ContentDialog
            {
                Title = DialogTitleChooseMusic,
                CloseButtonText = DialogButtonCancel,
                PrimaryButtonText = DialogButtonConfirm,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot,
            };

            ListView listView = new ListView
            {
                ItemsSource = this.MusicDialogViewModel.AvailableTracks,
                Height = MusicDialogHeight,
            };

            listView.ItemTemplate = (DataTemplate)Microsoft.UI.Xaml.Markup.XamlReader.Load(TrackItemDataTemplateXml);

            listView.SelectionChanged += (listSender, listEventArguments) =>
            {
                if (listView.SelectedItem is DataLayer.Models.MusicTrack track)
                {
                    this.MusicDialogViewModel.SelectedTrack = track;
                }
            };

            dialog.Content = listView;

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && this.MusicDialogViewModel.SelectedTrack != null)
            {
                this.ViewModel.ApplyMusicSelection(this.MusicDialogViewModel.SelectedTrack);
            }
        }
    }
}

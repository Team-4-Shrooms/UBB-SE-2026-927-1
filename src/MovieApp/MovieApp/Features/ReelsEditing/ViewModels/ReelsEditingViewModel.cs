namespace MovieApp.Features.ReelsEditing.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using MovieApp.Logic.Models;
    using MovieApp.Features.ReelsEditing.Models;
    using MovieApp.Features.ReelsEditing.Services;

    /// <summary>
    /// ViewModel for the reels editing interface.
    /// </summary>
    public partial class ReelsEditingViewModel : ObservableObject
    {
        private const int BaseVideoWidth = 1920;
        private const int BaseVideoHeight = 1080;
        private const double DefaultMusicDurationSeconds = 30.0;
        private const double DefaultMusicVolume = 80.0;
        private const double MinMusicDurationSeconds = 5.0;
        private const double MaxMusicDurationSeconds = 120.0;
        private const double MinMusicVolume = 0.0;
        private const double MaxMusicVolume = 100.0;
        private const double MinCropMargin = 0.0;
        private const double MaxCropMargin = 45.0;
        private const double PercentageDivisor = 100.0;
        private const double FullPercentage = 1.0;
        private const double MaxMusicStartTime = 300.0;
        private const double EmptyValue = 0.0;
        private const int EmptyRowsAffected = 0;

        private const string OptionCrop = "Crop";
        private const string OptionMusic = "Music";
        private const string JsonKeyX = "x";
        private const string JsonKeyY = "y";
        private const string JsonKeyWidth = "width";
        private const string JsonKeyHeight = "height";
        private const string JsonKeyMusicStartTime = "musicStartTime";
        private const string JsonKeyMusicDuration = "musicDuration";
        private const string JsonKeyMusicVolume = "musicVolume";

        private const string StatusMusicSelectedFormat = "Music selected: {0}";
        private const string StatusLoadMusicFailedFormat = "Failed to load music: {0}";
        private const string StatusSavingCrop = "Saving crop...";
        private const string StatusSavingMusic = "Saving music...";
        private const string StatusDeletingReel = "Deleting reel...";
        private const string StatusReelDeleted = "Reel deleted.";
        private const string StatusCropUpdatedFormat = "Crop dimensions updated successfully: X={0}, Y={1}, W={2}, H={3}.";
        private const string StatusSaveFailedFormat = "Save failed: {0}";
        private const string StatusDeleteFailedFormat = "Delete failed: {0}";
        private const string ErrorReelNotFoundFormat = "No reel found with ReelId={0}.";
        private const string ErrorCropPersistFailed = "Crop edits were not persisted correctly.";
        private const string ErrorMusicPersistFailed = "Music edits were not persisted correctly.";

        private readonly IReelRepository reelRepository;
        private readonly IVideoProcessingService videoProcessing;
        private readonly IAudioLibraryRepository audioLibrary;

        [ObservableProperty]
        private Reel? selectedReel;

        [ObservableProperty]
        private VideoEditMetadata currentEdits = new ();

        [ObservableProperty]
        private MusicTrack? selectedMusicTrack;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private bool isStatusSuccess = true;

        [ObservableProperty]
        private bool isSaving;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string selectedEditOption = string.Empty;

        [ObservableProperty]
        private ObservableCollection<MusicTrack> musicTracks = new ();

        [ObservableProperty]
        private bool isMusicChosen;

        [ObservableProperty]
        private double cropMarginLeft;

        [ObservableProperty]
        private double cropMarginTop;

        [ObservableProperty]
        private double cropMarginRight;

        [ObservableProperty]
        private double cropMarginBottom;

        [ObservableProperty]
        private double musicStartTime;

        [ObservableProperty]
        private double musicDuration = DefaultMusicDurationSeconds;

        [ObservableProperty]
        private double musicVolume = DefaultMusicVolume;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReelsEditingViewModel"/> class.
        /// </summary>
        /// <param name="reelRepository">The repository used to access reel data.</param>
        /// <param name="videoProcessing">The service used to process video editing operations.</param>
        /// <param name="audioLibrary">The library service used to fetch audio tracks.</param>
        public ReelsEditingViewModel(
            IReelRepository reelRepository,
            IVideoProcessingService videoProcessing,
            IAudioLibraryRepository audioLibrary)
        {
            this.reelRepository = reelRepository;
            this.videoProcessing = videoProcessing;
            this.audioLibrary = audioLibrary;
        }

        /// <summary>
        /// Event triggered when the crop editing mode is entered.
        /// </summary>
        public event Action? CropModeEntered;

        /// <summary>
        /// Event triggered when the crop editing mode is exited.
        /// </summary>
        public event Action? CropModeExited;

        /// <summary>
        /// Event triggered when a crop save operation begins.
        /// </summary>
        public event Action? CropSaveStarted;

        /// <summary>
        /// Event triggered when the cropped video has been successfully updated, providing the new video URL.
        /// </summary>
        public event Action<string>? CropVideoUpdated;

        /// <summary>
        /// Gets a value indicating whether there is a current status message to display.
        /// </summary>
        public bool HasStatusMessage => !string.IsNullOrWhiteSpace(this.StatusMessage);

        /// <summary>
        /// Loads the specified reel into the editing context.
        /// </summary>
        /// <param name="reel">The reel to load.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LoadReelAsync(Reel reel)
        {
            Reel freshReelData = await this.reelRepository.GetReelByIdAsync(reel.Id);
            if (freshReelData != null)
            {
                reel.VideoUrl = freshReelData.VideoUrl;
                reel.CropDataJson = freshReelData.CropDataJson;
                reel.BackgroundMusicId = freshReelData.BackgroundMusicId;
                reel.LastEditedAt = freshReelData.LastEditedAt;
            }

            this.SelectedReel = reel;
            this.CurrentEdits = new VideoEditMetadata();
            this.SelectedMusicTrack = null;
            this.IsMusicChosen = false;
            this.IsEditing = true;
            this.SelectedEditOption = string.Empty;
            this.CropMarginLeft = EmptyValue;
            this.CropMarginTop = EmptyValue;
            this.CropMarginRight = EmptyValue;
            this.CropMarginBottom = EmptyValue;
            this.MusicStartTime = EmptyValue;
            this.MusicDuration = DefaultMusicDurationSeconds;
            this.MusicVolume = DefaultMusicVolume;
            this.StatusMessage = string.Empty;
            this.IsStatusSuccess = true;

            this.LoadPersistedEditData(reel.CropDataJson, reel.BackgroundMusicId);

            if (reel.BackgroundMusicId.HasValue)
            {
                try
                {
                    MusicTrack track = await this.audioLibrary.GetTrackByIdAsync(reel.BackgroundMusicId.Value);
                    if (track != null)
                    {
                        this.SelectedMusicTrack = track;
                        this.NormalizeMusicTimingForSelectedTrack();
                    }
                }
                catch
                {
                    /* Non-fatal */
                }
            }
        }

        /// <summary>
        /// Applies the selected music track to the current reel edits.
        /// </summary>
        /// <param name="track">The music track to apply.</param>
        public void ApplyMusicSelection(MusicTrack track)
        {
            this.SelectedMusicTrack = track;
            this.CurrentEdits.SelectedMusicTrackId = track.Id;
            this.IsMusicChosen = true;
            this.MusicStartTime = EmptyValue;

            double reelDuration = (double)(this.SelectedReel?.FeatureDurationSeconds ?? (decimal)DefaultMusicDurationSeconds);
            this.MusicDuration = Math.Clamp(reelDuration, MinMusicDurationSeconds, MaxMusicDurationSeconds);

            this.NormalizeMusicTimingForSelectedTrack();
            this.IsStatusSuccess = true;
            this.StatusMessage = string.Format(StatusMusicSelectedFormat, track.TrackName);
        }

        private static int ReadInt(JsonElement rootElement, string propertyName, int fallbackValue)
        {
            if (rootElement.TryGetProperty(propertyName, out JsonElement jsonValue))
            {
                if (jsonValue.ValueKind == JsonValueKind.Number && jsonValue.TryGetInt32(out Int32 parsedInteger))
                {
                    return parsedInteger;
                }

                if (jsonValue.ValueKind == JsonValueKind.String && int.TryParse(jsonValue.GetString(), out Int32 parsedFromString))
                {
                    return parsedFromString;
                }
            }

            return fallbackValue;
        }

        private static double ReadDouble(JsonElement rootElement, string propertyName, double fallbackValue)
        {
            if (rootElement.TryGetProperty(propertyName, out JsonElement jsonValue))
            {
                if (jsonValue.ValueKind == JsonValueKind.Number && jsonValue.TryGetDouble(out Double parsedDouble))
                {
                    return parsedDouble;
                }

                if (jsonValue.ValueKind == JsonValueKind.String && double.TryParse(jsonValue.GetString(), out Double parsedFromString))
                {
                    return parsedFromString;
                }
            }

            return fallbackValue;
        }

        /// <summary>
        /// Partial method invoked when the status message property changes.
        /// </summary>
        /// <param name="value">The new status message value.</param>
        partial void OnStatusMessageChanged(string value)
        {
            this.OnPropertyChanged(nameof(this.HasStatusMessage));
        }

        [RelayCommand]
        private void SelectEditOption(string option)
        {
            if (this.SelectedEditOption == option)
            {
                if (this.SelectedEditOption == OptionCrop)
                {
                    this.CropModeExited?.Invoke();
                }

                this.SelectedEditOption = string.Empty;
                return;
            }

            if (this.SelectedEditOption == OptionCrop)
            {
                this.CropModeExited?.Invoke();
            }

            this.SelectedEditOption = option;

            if (option == OptionCrop)
            {
                this.CropModeEntered?.Invoke();
            }

            if (option == OptionMusic)
            {
                _ = this.LoadMusicTracksAsync();
            }
        }

        [RelayCommand]
        private void GoBack()
        {
            if (this.SelectedEditOption == OptionCrop)
            {
                this.CropModeExited?.Invoke();
            }

            this.SelectedReel = null;
            this.IsEditing = false;
            this.SelectedEditOption = string.Empty;
            this.StatusMessage = string.Empty;
            this.IsStatusSuccess = true;
        }

        private async Task LoadMusicTracksAsync()
        {
            try
            {
                IList<MusicTrack> tracks = await this.audioLibrary.GetAllTracksAsync();
                this.MusicTracks.Clear();
                foreach (MusicTrack musicTrack in tracks)
                {
                    this.MusicTracks.Add(musicTrack);
                }
            }
            catch (Exception exception)
            {
                this.IsStatusSuccess = false;
                this.StatusMessage = string.Format(StatusLoadMusicFailedFormat, exception.Message);
            }
        }

        [RelayCommand]
        private async Task SaveCropAsync()
        {
            if (this.SelectedReel == null)
            {
                return;
            }

            this.IsSaving = true;
            this.StatusMessage = StatusSavingCrop;
            this.IsStatusSuccess = true;

            try
            {
                this.CropSaveStarted?.Invoke();

                this.CurrentEdits.CropXCoordinate = (int)((this.CropMarginLeft / PercentageDivisor) * BaseVideoWidth);
                this.CurrentEdits.CropYCoordinate = (int)((this.CropMarginTop / PercentageDivisor) * BaseVideoHeight);
                this.CurrentEdits.CropWidth = (int)((FullPercentage - ((this.CropMarginLeft + this.CropMarginRight) / PercentageDivisor)) * BaseVideoWidth);
                this.CurrentEdits.CropHeight = (int)((FullPercentage - ((this.CropMarginTop + this.CropMarginBottom) / PercentageDivisor)) * BaseVideoHeight);

                string cropJson = this.CurrentEdits.ToCropDataJson();
                string processedVideoPath = await this.videoProcessing.ApplyCropAsync(this.SelectedReel.VideoUrl, cropJson);

                int rowsAffected = await this.reelRepository.UpdateReelEditsAsync(
                    this.SelectedReel.Id,
                    cropJson,
                    this.CurrentEdits.SelectedMusicTrackId,
                    processedVideoPath);

                if (rowsAffected == EmptyRowsAffected)
                {
                    throw new InvalidOperationException(string.Format(ErrorReelNotFoundFormat, this.SelectedReel.Id));
                }

                Reel persistedReel = await this.reelRepository.GetReelByIdAsync(this.SelectedReel.Id);
                if (persistedReel == null || persistedReel.CropDataJson != cropJson)
                {
                    throw new InvalidOperationException(ErrorCropPersistFailed);
                }

                this.SelectedReel.VideoUrl = persistedReel.VideoUrl;
                this.SelectedReel.CropDataJson = persistedReel.CropDataJson;
                this.SelectedReel.LastEditedAt = persistedReel.LastEditedAt;
                this.CropVideoUpdated?.Invoke(this.SelectedReel.VideoUrl);

                this.StatusMessage = string.Format(
                    StatusCropUpdatedFormat,
                    this.CurrentEdits.CropXCoordinate,
                    this.CurrentEdits.CropYCoordinate,
                    this.CurrentEdits.CropWidth,
                    this.CurrentEdits.CropHeight);
            }
            catch (Exception exception)
            {
                this.IsStatusSuccess = false;
                this.StatusMessage = string.Format(StatusSaveFailedFormat, exception.Message);
                this.CropVideoUpdated?.Invoke(this.SelectedReel.VideoUrl);
            }
            finally
            {
                this.IsSaving = false;
            }
        }

        [RelayCommand]
        private async Task SaveMusicAsync()
        {
            if (this.SelectedReel == null || this.SelectedMusicTrack == null)
            {
                return;
            }

            this.IsSaving = true;
            this.StatusMessage = StatusSavingMusic;
            this.IsStatusSuccess = true;

            try
            {
                this.CropSaveStarted?.Invoke();

                this.CurrentEdits.SelectedMusicTrackId = this.SelectedMusicTrack.Id;
                this.NormalizeMusicTimingForSelectedTrack();
                this.CurrentEdits.MusicStartTime = this.MusicStartTime;
                this.CurrentEdits.MusicDuration = this.MusicDuration;
                this.CurrentEdits.MusicVolume = this.MusicVolume;

                string processedVideoPath = await this.videoProcessing.MergeAudioAsync(
                    this.SelectedReel.VideoUrl,
                    this.SelectedMusicTrack.Id,
                    this.MusicStartTime,
                    this.MusicDuration,
                    this.MusicVolume);

                int rowsAffected = await this.reelRepository.UpdateReelEditsAsync(
                    this.SelectedReel.Id,
                    this.CurrentEdits.ToCropDataJson(),
                    this.SelectedMusicTrack.Id,
                    processedVideoPath);

                if (rowsAffected == EmptyRowsAffected)
                {
                    throw new InvalidOperationException(string.Format(ErrorReelNotFoundFormat, this.SelectedReel.Id));
                }

                Reel persistedReel = await this.reelRepository.GetReelByIdAsync(this.SelectedReel.Id);
                if (persistedReel == null || persistedReel.BackgroundMusicId != this.SelectedMusicTrack.Id)
                {
                    throw new InvalidOperationException(ErrorMusicPersistFailed);
                }

                this.SelectedReel.VideoUrl = persistedReel.VideoUrl;
                this.SelectedReel.CropDataJson = persistedReel.CropDataJson;
                this.SelectedReel.BackgroundMusicId = persistedReel.BackgroundMusicId;
                this.SelectedReel.LastEditedAt = persistedReel.LastEditedAt;
                this.CropVideoUpdated?.Invoke(this.SelectedReel.VideoUrl);

                this.StatusMessage = string.Format(StatusMusicSelectedFormat, this.SelectedMusicTrack.TrackName);
            }
            catch (Exception exception)
            {
                this.IsStatusSuccess = false;
                this.StatusMessage = string.Format(StatusSaveFailedFormat, exception.Message);
                this.CropVideoUpdated?.Invoke(this.SelectedReel.VideoUrl);
            }
            finally
            {
                this.IsSaving = false;
            }
        }

        [RelayCommand]
        private async Task DeleteReelAsync()
        {
            if (this.SelectedReel == null)
            {
                return;
            }

            try
            {
                this.StatusMessage = StatusDeletingReel;
                await this.reelRepository.DeleteReelAsync(this.SelectedReel.Id);
                this.StatusMessage = StatusReelDeleted;
                this.GoBack();
            }
            catch (Exception exception)
            {
                this.IsStatusSuccess = false;
                this.StatusMessage = string.Format(StatusDeleteFailedFormat, exception.Message);
            }
        }

        private void LoadPersistedEditData(string? cropDataJson, int? backgroundMusicId)
        {
            this.CurrentEdits.SelectedMusicTrackId = backgroundMusicId;
            if (backgroundMusicId.HasValue)
            {
                this.IsMusicChosen = true;
            }

            if (string.IsNullOrWhiteSpace(cropDataJson))
            {
                return;
            }

            try
            {
                using JsonDocument jsonDocument = JsonDocument.Parse(cropDataJson);
                JsonElement rootElement = jsonDocument.RootElement;

                this.CurrentEdits.CropXCoordinate = ReadInt(rootElement, JsonKeyX, (int)EmptyValue);
                this.CurrentEdits.CropYCoordinate = ReadInt(rootElement, JsonKeyY, (int)EmptyValue);
                this.CurrentEdits.CropWidth = ReadInt(rootElement, JsonKeyWidth, BaseVideoWidth);
                this.CurrentEdits.CropHeight = ReadInt(rootElement, JsonKeyHeight, BaseVideoHeight);
                this.CurrentEdits.MusicStartTime = Math.Max(EmptyValue, ReadDouble(rootElement, JsonKeyMusicStartTime, EmptyValue));
                this.CurrentEdits.MusicDuration = Math.Clamp(ReadDouble(rootElement, JsonKeyMusicDuration, DefaultMusicDurationSeconds), MinMusicDurationSeconds, MaxMusicDurationSeconds);
                this.CurrentEdits.MusicVolume = Math.Clamp(ReadDouble(rootElement, JsonKeyMusicVolume, DefaultMusicVolume), MinMusicVolume, MaxMusicVolume);

                this.CropMarginLeft = Math.Clamp((this.CurrentEdits.CropXCoordinate / (double)BaseVideoWidth) * PercentageDivisor, MinCropMargin, MaxCropMargin);
                this.CropMarginTop = Math.Clamp((this.CurrentEdits.CropYCoordinate / (double)BaseVideoHeight) * PercentageDivisor, MinCropMargin, MaxCropMargin);
                this.CropMarginRight = Math.Clamp(((BaseVideoWidth - (this.CurrentEdits.CropXCoordinate + this.CurrentEdits.CropWidth)) / (double)BaseVideoWidth) * PercentageDivisor, MinCropMargin, MaxCropMargin);
                this.CropMarginBottom = Math.Clamp(((BaseVideoHeight - (this.CurrentEdits.CropYCoordinate + this.CurrentEdits.CropHeight)) / (double)BaseVideoHeight) * PercentageDivisor, MinCropMargin, MaxCropMargin);

                this.MusicStartTime = this.CurrentEdits.MusicStartTime;
                this.MusicDuration = this.CurrentEdits.MusicDuration;
                this.MusicVolume = this.CurrentEdits.MusicVolume;
            }
            catch
            {
                // Keep defaults if previously stored JSON is malformed.
            }
        }

        private void NormalizeMusicTimingForSelectedTrack()
        {
            this.MusicStartTime = Math.Clamp(this.MusicStartTime, EmptyValue, MaxMusicStartTime);
            this.MusicDuration = Math.Clamp(this.MusicDuration, MinMusicDurationSeconds, MaxMusicDurationSeconds);
            this.MusicVolume = Math.Clamp(this.MusicVolume, MinMusicVolume, MaxMusicVolume);
        }
    }
}

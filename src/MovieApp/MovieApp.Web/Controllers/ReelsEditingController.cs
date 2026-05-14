namespace MovieApp.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using MovieApp.DataLayer.Models;
    using MovieApp.Logic.Features.ReelsEditing;
    using MovieApp.Logic.Interfaces.Services;
    using MovieApp.Web.ViewModels.ReelsEditing;

    public sealed class ReelsEditingController : Controller
    {
        private const int BaseVideoWidth = 1920;
        private const int BaseVideoHeight = 1080;
        private const double PercentageDivisor = 100.0;
        private const double FullPercentage = 1.0;
        private const double DefaultMusicDurationSeconds = 30.0;
        private const double DefaultMusicVolumePercentage = 80.0;
        private const double MinMusicDurationSeconds = 5.0;
        private const double MaxMusicDurationSeconds = 120.0;
        private const double MinMusicVolumePercentage = 0.0;
        private const double MaxMusicVolumePercentage = 100.0;
        private const double MinMusicStartTimeSeconds = 0.0;
        private const double MaxMusicStartTimeSeconds = 300.0;
        private const double MinCropMarginPercentage = 0.0;
        private const double MaxCropMarginPercentage = 45.0;
        private const string TempDataKeyStatusMessage = "StatusMessage";
        private const string TempDataKeyIsStatusSuccess = "IsStatusSuccess";

        private readonly IReelService reelService;
        private readonly IAudioLibraryService audioLibraryService;
        private readonly IVideoProcessingService videoProcessingService;
        private readonly ICurrentUserService currentUserService;
        private readonly string webApiBaseUrl;

        public ReelsEditingController(
            IReelService reelService,
            IAudioLibraryService audioLibraryService,
            IVideoProcessingService videoProcessingService,
            ICurrentUserService currentUserService,
            IConfiguration configuration)
        {
            this.reelService = reelService;
            this.audioLibraryService = audioLibraryService;
            this.videoProcessingService = videoProcessingService;
            this.currentUserService = currentUserService;
            this.webApiBaseUrl = configuration["WebApi:BaseUrl"]!.TrimEnd('/');
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Reels Editing";
            int currentUserId = this.currentUserService.UserId;
            IList<Reel> userReels = await this.reelService.GetUserReelsAsync(currentUserId);

            ReelsEditingGalleryViewModel galleryViewModel = new ReelsEditingGalleryViewModel
            {
                UserReels = userReels
                    .Select(reel => new ReelGalleryItem
                    {
                        ReelId = reel.Id,
                        Title = reel.Title,
                        Source = reel.Source,
                        ThumbnailUrl = this.ResolveMediaUrl(reel.ThumbnailUrl),
                        LastEditedAt = reel.LastEditedAt
                    })
                    .ToList()
            };

            return this.View(galleryViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int reelId)
        {
            Reel? selectedReel = await this.reelService.GetReelByIdAsync(reelId);
            if (selectedReel == null)
            {
                return this.NotFound();
            }

            List<MusicTrack> availableMusicTracks = await this.audioLibraryService.GetAllTracksAsync();
            MusicTrack? currentMusicTrack = selectedReel.BackgroundMusicId.HasValue
                ? availableMusicTracks.FirstOrDefault(track => track.Id == selectedReel.BackgroundMusicId.Value)
                : null;

            ReelsEditingEditorViewModel editorViewModel = this.BuildEditorViewModel(selectedReel, availableMusicTracks, currentMusicTrack);

            if (TempData[TempDataKeyStatusMessage] is string statusMessage)
            {
                editorViewModel.StatusMessage = statusMessage;
                editorViewModel.IsStatusSuccess = TempData[TempDataKeyIsStatusSuccess] is bool isSuccess && isSuccess;
            }

            ViewData["Title"] = $"Edit: {selectedReel.Title}";
            return this.View(editorViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCrop(SaveCropForm cropForm)
        {
            try
            {
                Reel? existingReel = await this.reelService.GetReelByIdAsync(cropForm.ReelId);
                if (existingReel == null)
                {
                    return this.NotFound();
                }

                double clampedMarginLeft = Math.Clamp(cropForm.CropMarginLeft, MinCropMarginPercentage, MaxCropMarginPercentage);
                double clampedMarginTop = Math.Clamp(cropForm.CropMarginTop, MinCropMarginPercentage, MaxCropMarginPercentage);
                double clampedMarginRight = Math.Clamp(cropForm.CropMarginRight, MinCropMarginPercentage, MaxCropMarginPercentage);
                double clampedMarginBottom = Math.Clamp(cropForm.CropMarginBottom, MinCropMarginPercentage, MaxCropMarginPercentage);

                int cropXCoordinate = (int)((clampedMarginLeft / PercentageDivisor) * BaseVideoWidth);
                int cropYCoordinate = (int)((clampedMarginTop / PercentageDivisor) * BaseVideoHeight);
                int cropWidth = (int)((FullPercentage - ((clampedMarginLeft + clampedMarginRight) / PercentageDivisor)) * BaseVideoWidth);
                int cropHeight = (int)((FullPercentage - ((clampedMarginTop + clampedMarginBottom) / PercentageDivisor)) * BaseVideoHeight);

                ReelEditData existingEditData = this.ParseExistingEditData(existingReel.CropDataJson);

                string updatedCropDataJson = JsonSerializer.Serialize(new
                {
                    x = cropXCoordinate,
                    y = cropYCoordinate,
                    width = cropWidth,
                    height = cropHeight,
                    musicStartTime = existingEditData.MusicStartTime,
                    musicDuration = existingEditData.MusicDuration,
                    musicVolume = existingEditData.MusicVolume
                });

                string resolvedVideoUrl = this.ResolveMediaUrl(existingReel.VideoUrl);
                string processedVideoPath = await this.videoProcessingService.ApplyCropAsync(resolvedVideoUrl, updatedCropDataJson);
                await this.reelService.UpdateReelEditsAsync(cropForm.ReelId, updatedCropDataJson, existingReel.BackgroundMusicId, processedVideoPath);

                TempData[TempDataKeyStatusMessage] = $"Crop saved: X={cropXCoordinate}, Y={cropYCoordinate}, W={cropWidth}, H={cropHeight}.";
                TempData[TempDataKeyIsStatusSuccess] = true;
            }
            catch (Exception saveException)
            {
                TempData[TempDataKeyStatusMessage] = $"Save failed: {saveException.Message}";
                TempData[TempDataKeyIsStatusSuccess] = false;
            }

            return this.RedirectToAction(nameof(this.Edit), new { reelId = cropForm.ReelId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMusic(SaveMusicForm musicForm)
        {
            try
            {
                Reel? existingReel = await this.reelService.GetReelByIdAsync(musicForm.ReelId);
                if (existingReel == null)
                {
                    return this.NotFound();
                }

                double clampedStartTime = Math.Clamp(musicForm.MusicStartTime, MinMusicStartTimeSeconds, MaxMusicStartTimeSeconds);
                double clampedDuration = Math.Clamp(musicForm.MusicDuration, MinMusicDurationSeconds, MaxMusicDurationSeconds);
                double clampedVolume = Math.Clamp(musicForm.MusicVolume, MinMusicVolumePercentage, MaxMusicVolumePercentage);

                ReelEditData existingEditData = this.ParseExistingEditData(existingReel.CropDataJson);

                string updatedCropDataJson = JsonSerializer.Serialize(new
                {
                    x = existingEditData.CropXCoordinate,
                    y = existingEditData.CropYCoordinate,
                    width = existingEditData.CropWidth,
                    height = existingEditData.CropHeight,
                    musicStartTime = clampedStartTime,
                    musicDuration = clampedDuration,
                    musicVolume = clampedVolume
                });

                string resolvedVideoUrl = this.ResolveMediaUrl(existingReel.VideoUrl);
                string processedVideoPath = await this.videoProcessingService.MergeAudioAsync(
                    resolvedVideoUrl,
                    musicForm.MusicTrackId,
                    clampedStartTime,
                    clampedDuration,
                    clampedVolume);

                await this.reelService.UpdateReelEditsAsync(musicForm.ReelId, updatedCropDataJson, musicForm.MusicTrackId, processedVideoPath);

                TempData[TempDataKeyStatusMessage] = "Music track saved successfully.";
                TempData[TempDataKeyIsStatusSuccess] = true;
            }
            catch (Exception saveException)
            {
                TempData[TempDataKeyStatusMessage] = $"Save failed: {saveException.Message}";
                TempData[TempDataKeyIsStatusSuccess] = false;
            }

            return this.RedirectToAction(nameof(this.Edit), new { reelId = musicForm.ReelId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReel(int reelId)
        {
            await this.reelService.DeleteReelAsync(reelId);
            return this.RedirectToAction(nameof(this.Index));
        }

        private string ResolveMediaUrl(string mediaPath)
        {
            return mediaPath.StartsWith("/") ? this.webApiBaseUrl + mediaPath : mediaPath;
        }

        private ReelsEditingEditorViewModel BuildEditorViewModel(
            Reel selectedReel,
            List<MusicTrack> availableMusicTracks,
            MusicTrack? currentMusicTrack)
        {
            ReelEditData existingEditData = this.ParseExistingEditData(selectedReel.CropDataJson);

            double cropMarginLeft = Math.Clamp(
                (existingEditData.CropXCoordinate / (double)BaseVideoWidth) * PercentageDivisor,
                MinCropMarginPercentage, MaxCropMarginPercentage);

            double cropMarginTop = Math.Clamp(
                (existingEditData.CropYCoordinate / (double)BaseVideoHeight) * PercentageDivisor,
                MinCropMarginPercentage, MaxCropMarginPercentage);

            double cropMarginRight = Math.Clamp(
                ((BaseVideoWidth - (existingEditData.CropXCoordinate + existingEditData.CropWidth)) / (double)BaseVideoWidth) * PercentageDivisor,
                MinCropMarginPercentage, MaxCropMarginPercentage);

            double cropMarginBottom = Math.Clamp(
                ((BaseVideoHeight - (existingEditData.CropYCoordinate + existingEditData.CropHeight)) / (double)BaseVideoHeight) * PercentageDivisor,
                MinCropMarginPercentage, MaxCropMarginPercentage);

            List<MusicTrackViewModel> musicTrackViewModels = availableMusicTracks
                .Select(track => new MusicTrackViewModel
                {
                    TrackId = track.Id,
                    TrackName = track.TrackName,
                    Author = track.Author,
                    FormattedDuration = track.FormattedDuration,
                    AudioUrl = this.ResolveMediaUrl(track.AudioUrl)
                })
                .ToList();

            return new ReelsEditingEditorViewModel
            {
                ReelId = selectedReel.Id,
                ReelTitle = selectedReel.Title,
                VideoUrl = this.ResolveMediaUrl(selectedReel.VideoUrl),
                AvailableMusicTracks = musicTrackViewModels,
                CurrentMusicTrackName = currentMusicTrack?.TrackName,
                SelectedMusicTrackId = selectedReel.BackgroundMusicId,
                CropMarginLeft = cropMarginLeft,
                CropMarginTop = cropMarginTop,
                CropMarginRight = cropMarginRight,
                CropMarginBottom = cropMarginBottom,
                MusicStartTime = existingEditData.MusicStartTime,
                MusicDuration = existingEditData.MusicDuration,
                MusicVolume = existingEditData.MusicVolume
            };
        }

        private ReelEditData ParseExistingEditData(string? cropDataJson)
        {
            ReelEditData editData = new ReelEditData
            {
                CropWidth = BaseVideoWidth,
                CropHeight = BaseVideoHeight,
                MusicDuration = DefaultMusicDurationSeconds,
                MusicVolume = DefaultMusicVolumePercentage
            };

            if (string.IsNullOrWhiteSpace(cropDataJson))
            {
                return editData;
            }

            try
            {
                using JsonDocument jsonDocument = JsonDocument.Parse(cropDataJson);
                JsonElement rootElement = jsonDocument.RootElement;

                editData.CropXCoordinate = ReadIntFromJson(rootElement, "x", 0);
                editData.CropYCoordinate = ReadIntFromJson(rootElement, "y", 0);
                editData.CropWidth = ReadIntFromJson(rootElement, "width", BaseVideoWidth);
                editData.CropHeight = ReadIntFromJson(rootElement, "height", BaseVideoHeight);
                editData.MusicStartTime = ReadDoubleFromJson(rootElement, "musicStartTime", 0.0);
                editData.MusicDuration = Math.Clamp(ReadDoubleFromJson(rootElement, "musicDuration", DefaultMusicDurationSeconds), MinMusicDurationSeconds, MaxMusicDurationSeconds);
                editData.MusicVolume = Math.Clamp(ReadDoubleFromJson(rootElement, "musicVolume", DefaultMusicVolumePercentage), MinMusicVolumePercentage, MaxMusicVolumePercentage);
            }
            catch
            {
                // Keep defaults if stored JSON is malformed.
            }

            return editData;
        }

        private static int ReadIntFromJson(JsonElement rootElement, string propertyName, int fallbackValue)
        {
            if (rootElement.TryGetProperty(propertyName, out JsonElement jsonValue))
            {
                if (jsonValue.ValueKind == JsonValueKind.Number && jsonValue.TryGetInt32(out int parsedInteger))
                {
                    return parsedInteger;
                }

                if (jsonValue.ValueKind == JsonValueKind.String && int.TryParse(jsonValue.GetString(), out int parsedFromString))
                {
                    return parsedFromString;
                }
            }

            return fallbackValue;
        }

        private static double ReadDoubleFromJson(JsonElement rootElement, string propertyName, double fallbackValue)
        {
            if (rootElement.TryGetProperty(propertyName, out JsonElement jsonValue))
            {
                if (jsonValue.ValueKind == JsonValueKind.Number && jsonValue.TryGetDouble(out double parsedDouble))
                {
                    return parsedDouble;
                }

                if (jsonValue.ValueKind == JsonValueKind.String && double.TryParse(jsonValue.GetString(), out double parsedFromString))
                {
                    return parsedFromString;
                }
            }

            return fallbackValue;
        }

        private sealed class ReelEditData
        {
            public int CropXCoordinate { get; set; }
            public int CropYCoordinate { get; set; }
            public int CropWidth { get; set; }
            public int CropHeight { get; set; }
            public double MusicStartTime { get; set; }
            public double MusicDuration { get; set; }
            public double MusicVolume { get; set; }
        }
    }
}

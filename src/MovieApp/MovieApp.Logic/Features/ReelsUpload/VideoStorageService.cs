using System;
using System.IO;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;

namespace MovieApp.Logic.Features.ReelsUpload
{
    /// <summary>
    /// Concrete implementation of IVideoStorageService.
    /// </summary>
    public class VideoStorageService : IVideoStorageService
    {
        private readonly IVideoStorageRepository memoryRepository;
        private readonly string blobStorageDirectory;

        private const string VideoFileExtension = ".mp4";
        private const string EmptyURL = "";
        private const string UploadSource = "upload";
        private const int NullId = 0;
        private const double MaximumReelDurationSeconds = 60.0;

        public VideoStorageService(IVideoStorageRepository memoryRepository)
        {
            this.memoryRepository = memoryRepository;

            blobStorageDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MeioAI",
                "Videos");

            if (!Directory.Exists(blobStorageDirectory))
            {
                Directory.CreateDirectory(blobStorageDirectory);
            }
        }

        public async Task<bool> ValidateVideoAsync(string localFilePath)
        {
            if (string.IsNullOrWhiteSpace(localFilePath) || !File.Exists(localFilePath)) return false;

            String fileExtension = Path.GetExtension(localFilePath).ToLowerInvariant();
            if (fileExtension != VideoFileExtension) return false;

            return await Task.FromResult(true);
        }

        public async Task<Reel> UploadVideoAsync(ReelUploadRequest request)
        {
            if (!File.Exists(request.LocalFilePath))
                throw new FileNotFoundException("The selected video file could not be found.", request.LocalFilePath);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.LocalFilePath);
            string destinationBlobPath = Path.Combine(blobStorageDirectory, fileName);

            await Task.Run(() => File.Copy(request.LocalFilePath, destinationBlobPath, overwrite: true));

            double computedDurationSeconds = 15.0;

            Reel newReel = new Reel
            {
                Movie = new Movie { Id = request.MovieId ?? NullId },
                CreatorUser = new User { Id = request.UploaderUserId },
                VideoUrl = destinationBlobPath,
                ThumbnailUrl = EmptyURL,
                Title = request.Title,
                Caption = request.Caption,
                FeatureDurationSeconds = (decimal)computedDurationSeconds,
                Source = UploadSource
            };

            return await memoryRepository.InsertReelAsync(newReel);
        }
    }
}

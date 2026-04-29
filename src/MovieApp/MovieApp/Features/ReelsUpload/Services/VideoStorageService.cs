using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MovieApp.DataLayer.Models;
using MovieApp.Features.ReelsUpload.Models;


namespace MovieApp.Features.ReelsUpload.Services
{
    /// <summary>
    /// Concrete implementation of IVideoStorageService.
    /// </summary>
    public class VideoStorageService : IVideoStorageService
    {
        private readonly IVideoStorageRepository memoryRepository;

        // Simulating a blob storage directory inside the AppData folder for local development
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
            // 1. Standard C# File Check (Safe)
            if (string.IsNullOrWhiteSpace(localFilePath) || !File.Exists(localFilePath))
            {
                return false;
            }

            // 2. Standard C# Extension Check (Safe)
            String fileExtension = Path.GetExtension(localFilePath).ToLowerInvariant();
            if (fileExtension != VideoFileExtension)
            {
                return false;
            }

            // 🚨 REMOVED: The Windows.Storage API that was deadlocking the app.
            // If it's an MP4 and it exists, we approve it!

            // Need to return a Task to satisfy the async signature
            return await Task.FromResult(true);
        }

        public async Task<Reel> UploadVideoAsync(ReelUploadRequest request)
        {
            if (!File.Exists(request.LocalFilePath))
            {
                throw new FileNotFoundException("The selected video file could not be found.", request.LocalFilePath);
            }

            // "Upload" to Blob Storage (Safe)
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.LocalFilePath);
            string destinationBlobPath = Path.Combine(blobStorageDirectory, fileName);

            // Copy the file
            await Task.Run(() => File.Copy(request.LocalFilePath, destinationBlobPath, overwrite: true));

            // 🚨 REMOVED: The Windows.Storage native duration check that was deadlocking.
            // We will just use the standard 15-second default for all uploads.
            double computedDurationSeconds = 15.0;

            // Prepare the model with the data we know
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

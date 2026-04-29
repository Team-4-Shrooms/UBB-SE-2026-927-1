using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MovieApp.DataLayer.Models;
using MovieApp.Features.ReelsUpload.Models;
using Windows.Storage;
using Windows.Storage.FileProperties;


namespace MovieApp.Features.ReelsUpload.Services
{
    /// <summary>
    /// Concrete implementation of IVideoStorageService.
    /// Owner: Alex
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
            if (string.IsNullOrWhiteSpace(localFilePath) || !File.Exists(localFilePath))
            {
                return false;
            }

            String fileExtension = Path.GetExtension(localFilePath).ToLowerInvariant();
            if (fileExtension != VideoFileExtension)
            {
                return false;
            }

            try
            {
                StorageFile storageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(localFilePath);
                VideoProperties videoProperties = await storageFile.Properties.GetVideoPropertiesAsync();

                if (videoProperties.Duration.TotalSeconds > MaximumReelDurationSeconds)
                {
                    return false; // Video is too long
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<Reel> UploadVideoAsync(ReelUploadRequest request)
        {
            if (!File.Exists(request.LocalFilePath))
            {
                throw new FileNotFoundException("The selected video file could not be found.", request.LocalFilePath);
            }

            // "Upload" to Blob Storage
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.LocalFilePath);
            string destinationBlobPath = Path.Combine(blobStorageDirectory, fileName);
            await Task.Run(() => File.Copy(request.LocalFilePath, destinationBlobPath, overwrite: true));

            // Compute TRUE duration natively
            double computedDurationSeconds = 0;
            try
            {
                StorageFile storageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(request.LocalFilePath);
                VideoProperties videoProps = await storageFile.Properties.GetVideoPropertiesAsync();
                computedDurationSeconds = videoProps.Duration.TotalSeconds;
            }
            catch
            {
                computedDurationSeconds = 15.0; // Fallback
            }

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

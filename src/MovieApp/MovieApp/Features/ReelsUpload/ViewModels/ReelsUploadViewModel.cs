using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Features.ReelsUpload;

namespace MovieApp.Features.ReelsUpload.ViewModels
{
    /// <summary>
    /// ViewModel for the Reels Upload page.
    /// </summary>
    public partial class ReelsUploadViewModel : ObservableObject
    {
        private readonly IVideoStorageService videoStorageService;
        private readonly IMovieService movieService;

        private const string UntitledName = "Untitled Reel";
        private const string VideoFileExtension = ".mp4";

        private List<Movie> _allMovies = new List<Movie>();

        public ObservableCollection<Movie> SuggestedMovies { get; }

        public ReelsUploadViewModel(
            IVideoStorageService videoStorageService,
            IMovieService movieService)
        {
            this.videoStorageService = videoStorageService;
            this.movieService = movieService;
            SuggestedMovies = new ObservableCollection<Movie>();

            _ = LoadMoviesAsync();
        }

        [ObservableProperty]
        private string pageTitle = "Reels Upload";

        [ObservableProperty]
        private string statusMessage = "Ready to upload.";

        private const int CurrentUserID = 1;

        [ObservableProperty]
        private string reelTitle = string.Empty;

        [ObservableProperty]
        private string reelCaption = string.Empty;

        [ObservableProperty]
        private Movie? linkedMovie;

        [ObservableProperty]
        private string localVideoFilePath = string.Empty;

        private async Task LoadMoviesAsync()
        {
            try
            {
                var movies = await movieService.GetAllMoviesAsync();
                _allMovies = movies.ToList();
            }
            catch (Exception exception)
            {
                StatusMessage = $"Failed to load movies: {exception.Message}";
            }
        }

        [RelayCommand]
        private async Task SelectVideoFileAsync()
        {
            Windows.Storage.Pickers.FileOpenPicker filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.FileTypeFilter.Add(VideoFileExtension);

            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, windowHandle);

            Windows.Storage.StorageFile selectedMovieFile = await filePicker.PickSingleFileAsync();
            if (selectedMovieFile != null)
            {
                string tempDirectory = Path.GetTempPath();
                string tempFilePath = Path.Combine(tempDirectory, selectedMovieFile.Name);

                File.Copy(selectedMovieFile.Path, tempFilePath, overwrite: true);

                LocalVideoFilePath = tempFilePath;
            }
        }

        [RelayCommand]
        private async Task UploadReelAsync()
        {
            if (string.IsNullOrWhiteSpace(LocalVideoFilePath))
            {
                StatusMessage = "Please select a video first!";
                return;
            }

            if (string.IsNullOrWhiteSpace(ReelTitle))
            {
                StatusMessage = "Please enter a title for the reel!";
                return;
            }

            if (LinkedMovie == null)
            {
                StatusMessage = "Please link a movie to the reel!";
                return;
            }

            StatusMessage = "Validating video format...";

            try
            {
                // FIX FOR PART 4: Validate locally instead of calling the proxy service
                bool isValid = ValidateVideoLocally(LocalVideoFilePath);

                if (!isValid)
                {
                    StatusMessage = "Invalid file! Must be a non-empty MP4 file.";
                    return;
                }

                StatusMessage = "Uploading to Blob Storage & saving metadata...";

                ReelUploadRequest request = new ReelUploadRequest
                {
                    LocalFilePath = LocalVideoFilePath,
                    Title = ReelTitle,
                    Caption = ReelCaption ?? string.Empty,
                    UploaderUserId = CurrentUserID,
                    MovieId = LinkedMovie.Id
                };

                Reel savedReel = await videoStorageService.UploadVideoAsync(request);

                StatusMessage = $"Success! Reel uploaded with ID {savedReel.Id}.";
                LocalVideoFilePath = string.Empty;
                ReelTitle = string.Empty;
                ReelCaption = string.Empty;
                LinkedMovie = null;
            }
            catch (Exception exception)
            {
                StatusMessage = $"Upload Failed: {exception.Message}";
            }
        }

        private bool ValidateVideoLocally(string localFilePath)
        {
            if (string.IsNullOrWhiteSpace(localFilePath) || !File.Exists(localFilePath))
                return false;

            string fileExtension = Path.GetExtension(localFilePath).ToLowerInvariant();
            return fileExtension == VideoFileExtension;
        }

        [RelayCommand]
        private void SelectMovie(Movie movieToSelect)
        {
            LinkedMovie = movieToSelect;
        }

        [RelayCommand]
        private void SearchMovie(string partialMovieName)
        {
            SuggestedMovies.Clear();

            if (string.IsNullOrWhiteSpace(partialMovieName))
            {
                return;
            }

            var filteredMovies = _allMovies
                .Where(movie => movie.Title.Contains(partialMovieName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (Movie movie in filteredMovies)
            {
                SuggestedMovies.Add(movie);
            }
        }
    }
}

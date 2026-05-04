using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Features.ReelsUpload;
using System.Runtime.InteropServices;

namespace MovieApp.Features.ReelsUpload.ViewModels
{
    /// <summary>
    /// ViewModel for the Reels Upload page.
    /// </summary>
    public partial class ReelsUploadViewModel : ObservableObject
    {
        //private readonly IAppWindowContext appWindowContext;
        private readonly IVideoStorageService videoStorageService;
        private readonly IMovieRepository movieRepository;

        private const string UntitledName = "Untitled Reel";

        public ReelsUploadViewModel(
            IVideoStorageService videoStorageService,
            IMovieRepository movieRepository)
        {
            //this.appWindowContext = appWindowContext;
            this.videoStorageService = videoStorageService;
            this.movieRepository = movieRepository;
            SuggestedMovies = new ObservableCollection<Movie>();
        }

        public ObservableCollection<Movie> SuggestedMovies { get; }

        [ObservableProperty]
        private string pageTitle = "Reels Upload";

        [ObservableProperty]
        private string statusMessage = "Ready to upload.";

        // TODO: Replace with actual authenticated user ID later
        private const int CurrentUserID = 1;

        [ObservableProperty]
        private string reelTitle = string.Empty;

        [ObservableProperty]
        private string reelCaption = string.Empty;

        [ObservableProperty]
        private Movie? linkedMovie;

        [ObservableProperty]
        private string localVideoFilePath = string.Empty;

        private const string VideoFileExtension = ".mp4";

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
                LocalVideoFilePath = selectedMovieFile.Path;
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
                bool isValid = await videoStorageService.ValidateVideoAsync(LocalVideoFilePath);

                // 1. If invalid, show error and STOP.
                if (!isValid)
                {
                    StatusMessage = "Invalid file! Must be a non-empty MP4 file\nno longer than 60 seconds.";
                    return;
                }

                // 2. If valid, proceed with the upload!
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
            catch (Exception ex)
            {
                string errorMessage = $"Upload Failed: {ex.Message}";
                StatusMessage = errorMessage;
            }
        }

        [RelayCommand]
        private void SelectMovie(Movie movieToSelect)
        {
            LinkedMovie = movieToSelect;
        }

        [RelayCommand]
        private async Task SearchMovieAsync(string partialMovieName)
        {
            if (string.IsNullOrWhiteSpace(partialMovieName))
            {
                SuggestedMovies.Clear();
                return;
            }

            try
            {
                const int DefaultSearchLimit = 10;

                System.Collections.Generic.List<Movie> searchResults = await movieRepository.SearchMoviesAsync(partialMovieName, DefaultSearchLimit);

                SuggestedMovies.Clear();
                foreach (Movie movie in searchResults)
                {
                    SuggestedMovies.Add(movie);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Search Error: {ex.Message}";
            }
        }
    }
}

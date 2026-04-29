namespace MovieApp.Features.ReelsEditing.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using MovieApp.DataLayer.Models;
    using MovieApp.Features.ReelsEditing.Services;

    /// <summary>
    /// ViewModel for the reel gallery view.
    /// </summary>
    public partial class ReelGalleryViewModel : ObservableObject
    {
        private const int CurrentUserId = 1;
        private const int EmptyReelCount = 0;

        private const string DefaultStatusMessage = "Select a reel to edit.";
        private const string LoadingMessage = "Loading reels...";
        private const string ReelsFoundMessageFormat = "{0} reel(s) found.";
        private const string NoReelsMessage = "No reels uploaded yet. Upload a reel first.";
        private const string ErrorLoadingMessageFormat = "Error loading reels: {0}";

        private readonly IReelRepository reelRepository;

        [ObservableProperty]
        private ObservableCollection<Reel> userReels = new ();

        [ObservableProperty]
        private Reel? selectedReel;

        [ObservableProperty]
        private string statusMessage = DefaultStatusMessage;

        [ObservableProperty]
        private bool isLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReelGalleryViewModel"/> class.
        /// </summary>
        /// <param name="reelRepository">The repository used to fetch user reels.</param>
        public ReelGalleryViewModel(IReelRepository reelRepository)
        {
            this.reelRepository = reelRepository;
        }

        /// <summary>
        /// Ensures that the reels are loaded if they haven't been loaded yet.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task EnsureLoadedAsync()
        {
            if (!this.IsLoaded)
            {
                await this.LoadReelsAsync();
            }
        }

        /// <summary>
        /// Loads the reels from the repository.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task LoadReelsAsync()
        {
            this.StatusMessage = LoadingMessage;
            try
            {
                IList<Reel> reels = await this.reelRepository.GetUserReelsAsync(CurrentUserId);
                this.UserReels.Clear();
                foreach (Reel reel in reels)
                {
                    this.UserReels.Add(reel);
                }

                this.IsLoaded = true;

                this.StatusMessage = this.UserReels.Count > EmptyReelCount
                    ? string.Format(ReelsFoundMessageFormat, this.UserReels.Count)
                    : NoReelsMessage;
            }
            catch (Exception exception)
            {
                this.StatusMessage = string.Format(ErrorLoadingMessageFormat, exception.Message);
            }
        }
    }
}

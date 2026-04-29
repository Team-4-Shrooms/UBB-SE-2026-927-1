using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using MovieApp.Logic.Models;
using MovieApp.Features.MovieTournament.Services;

namespace MovieApp.Features.MovieTournament.ViewModels
{
    /// <summary>
    /// View model for the tournament setup page, allowing the user to configure
    /// the pool size and start a new tournament.
    /// </summary>
    public partial class TournamentSetupViewModel : ObservableObject
    {
        private const int CurrentUserId = 1;
        private const int MinimumPoolSize = 4;
        private const int BackgroundImageCount = 4;

        private const string FallbackPoster1 = "https://image.tmdb.org/t/p/w500/3bhkrj58Vtu7enYsRolD1fZdja1.jpg";
        private const string FallbackPoster2 = "https://media.themoviedb.org/t/p/w600_and_h900_face/qJ2tW6WMUDux911r6m7haRef0WH.jpg";
        private const string FallbackPoster3 = "https://media.themoviedb.org/t/p/w600_and_h900_face/q2qXg4OmJgm0qGaBYLdXzP8nHPy.jpg";
        private const string FallbackPoster4 = "https://media.themoviedb.org/t/p/w600_and_h900_face/nrmXQ0zcZUL8jFLrakWc90IR8z9.jpg";

        private readonly ITournamentLogicService tournamentLogicService;
        private readonly IMovieTournamentRepository tournamentRepository;

        [ObservableProperty]
        private int poolSize = MinimumPoolSize;

        [ObservableProperty]
        private int maxPoolSize = MinimumPoolSize;

        [ObservableProperty]
        private string setupErrorMessage = string.Empty;

        [ObservableProperty]
        private string? backgroundPoster1;

        [ObservableProperty]
        private string? backgroundPoster2;

        [ObservableProperty]
        private string? backgroundPoster3;

        [ObservableProperty]
        private string? backgroundPoster4;

        /// <summary>
        /// Raised when the tournament has been successfully started
        /// and the view should navigate to the match page.
        /// </summary>
        public event EventHandler? TournamentStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentSetupViewModel"/> class
        /// and immediately begins loading pool size and background poster data.
        /// </summary>
        /// <param name="tournamentLogicService">The service managing tournament bracket logic.</param>
        /// <param name="tournamentRepository">The repository used to load pool data.</param>
        public TournamentSetupViewModel(
            ITournamentLogicService tournamentLogicService,
            IMovieTournamentRepository tournamentRepository)
        {
            this.tournamentLogicService = tournamentLogicService;
            this.tournamentRepository = tournamentRepository;
            _ = this.LoadSetupDataAsync();
        }

        /// <summary>
        /// Converts a poster URL into an <see cref="ImageSource"/> suitable for binding.
        /// Returns <see langword="null"/> if the URL is null, empty, or malformed.
        /// </summary>
        /// <param name="posterUrl">The URL of the poster image.</param>
        /// <returns>A <see cref="BitmapImage"/>, or <see langword="null"/> if the URL is invalid.</returns>
        public ImageSource? GetImageSource(string? posterUrl)
        {
            if (string.IsNullOrWhiteSpace(posterUrl))
            {
                return null;
            }

            try
            {
                return new BitmapImage(new Uri(posterUrl));
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// Validates the selected pool size and starts the tournament,
        /// raising <see cref="TournamentStarted"/> on success.
        /// Sets <see cref="SetupErrorMessage"/> if validation fails or the service throws.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous start operation.</returns>
        [RelayCommand]
        public async Task StartTournamentAsync()
        {
            if (this.PoolSize < MinimumPoolSize)
            {
                this.SetupErrorMessage = $"Pool size must be at least {MinimumPoolSize}.\nIf you don't have enough, go like some movies!";
                return;
            }

            if (this.PoolSize > this.MaxPoolSize)
            {
                this.SetupErrorMessage = $"Pool size cannot exceed {this.MaxPoolSize}.";
                return;
            }

            this.SetupErrorMessage = string.Empty;

            try
            {
                await this.tournamentLogicService.StartTournamentAsync(CurrentUserId, this.PoolSize);
                this.TournamentStarted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                this.SetupErrorMessage = $"Failed to start tournament: {exception.Message}";
            }
        }

        /// <summary>
        /// Loads the maximum pool size and up to four background poster URLs from the repository.
        /// Falls back to hardcoded themoviedb.org images if fewer than four movies are available.
        /// Sets <see cref="SetupErrorMessage"/> if the repository call fails.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous load operation.</returns>
        public async Task LoadSetupDataAsync()
        {
            try
            {
                this.MaxPoolSize = await this.tournamentRepository.GetTournamentPoolSizeAsync(CurrentUserId);

                List<Movie> backgroundMovies = await this.tournamentRepository.GetTournamentPoolAsync(
                    CurrentUserId, BackgroundImageCount);

                List<string> posters = new List<string?>
                {
                    FallbackPoster1,
                    FallbackPoster2,
                    FallbackPoster3,
                    FallbackPoster4,
                };

                for (int i = 0; i < backgroundMovies.Count && i < BackgroundImageCount; i++)
                {
                    if (!string.IsNullOrWhiteSpace(backgroundMovies[i].PosterUrl))
                    {
                        posters[i] = backgroundMovies[i].PosterUrl;
                    }
                }

                this.BackgroundPoster1 = posters[0];
                this.BackgroundPoster2 = posters[1];
                this.BackgroundPoster3 = posters[2];
                this.BackgroundPoster4 = posters[3];
            }
            catch (Exception exception)
            {
                this.SetupErrorMessage = $"Error loading data: {exception.Message}";
            }
        }
    }
}

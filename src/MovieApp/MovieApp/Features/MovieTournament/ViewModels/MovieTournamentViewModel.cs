using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using MovieApp.Logic.Models;
using MovieApp.Features.MovieTournament.Services;
using MovieApp.Features.MovieTournament.Models;

namespace MovieApp.Features.MovieTournament.ViewModels
{
    /// <summary>
    /// Unified view model for the movie tournament feature.
    /// Manages three view states: Setup (0), Match (1), and Winner (2),
    /// driven by a single <see cref="CurrentViewState"/> property.
    /// Call <see cref="InitializeAsync"/> after construction to restore
    /// the correct state based on the current tournament status.
    /// </summary>
    public partial class MovieTournamentViewModel : ObservableObject
    {
        private const int SetupViewState = 0;
        private const int MatchViewState = 1;
        private const int WinnerViewState = 2;
        private const int MinimumPoolSize = 4;
        private const int BackgroundImageCount = 4;
        private const int CurrentUserId = 1;

        private const string FallbackPoster1 = "https://image.tmdb.org/t/p/w500/3bhkrj58Vtu7enYsRolD1fZdja1.jpg";
        private const string FallbackPoster2 = "https://media.themoviedb.org/t/p/w600_and_h900_face/qJ2tW6WMUDux911r6m7haRef0WH.jpg";
        private const string FallbackPoster3 = "https://media.themoviedb.org/t/p/w600_and_h900_face/q2qXg4OmJgm0qGaBYLdXzP8nHPy.jpg";
        private const string FallbackPoster4 = "https://media.themoviedb.org/t/p/w600_and_h900_face/nrmXQ0zcZUL8jFLrakWc90IR8z9.jpg";

        private readonly ITournamentLogicService tournamentLogicService;
        private readonly IMovieTournamentRepository tournamentRepository;

        [ObservableProperty]
        private int currentViewState = SetupViewState;

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

        [ObservableProperty]
        private Movie? movieOptionA;

        [ObservableProperty]
        private Movie? movieOptionB;

        [ObservableProperty]
        private string roundDisplay = string.Empty;

        [ObservableProperty]
        private Movie? winnerMovie;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieTournamentViewModel"/> class.
        /// Dependencies are injected but no initialization logic runs here.
        /// Call <see cref="InitializeAsync"/> explicitly after construction.
        /// </summary>
        /// <param name="tournamentLogicService">Service that manages tournament logic and state.</param>
        /// <param name="tournamentRepository">Repository used to load pool size and background posters.</param>
        public MovieTournamentViewModel(
            ITournamentLogicService tournamentLogicService,
            IMovieTournamentRepository tournamentRepository)
        {
            this.tournamentLogicService = tournamentLogicService;
            this.tournamentRepository = tournamentRepository;
        }

        /// <summary>
        /// Restores the view model to the correct state based on the current tournament status.
        /// Should be called once after construction, typically from the view's loaded event or page navigation.
        /// <list type="bullet">
        ///   <item>If a tournament is active, transitions to the match view and displays the current match.</item>
        ///   <item>If a tournament is complete, transitions to the winner view and sets the winner movie.</item>
        ///   <item>Otherwise, loads setup data (pool size and background posters) for the setup view.</item>
        /// </list>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous initialization operation.</returns>
        public async Task InitializeAsync()
        {
            if (this.tournamentLogicService.IsTournamentActive)
            {
                this.UpdateCurrentMatchDisplay();
                this.CurrentViewState = MatchViewState;
            }
            else if (this.tournamentLogicService.IsTournamentComplete())
            {
                this.WinnerMovie = this.tournamentLogicService.GetFinalWinner();
                this.CurrentViewState = WinnerViewState;
            }
            else
            {
                await this.LoadSetupDataAsync();
            }
        }

        /// <summary>
        /// Converts a poster URL string into a <see cref="BitmapImage"/> suitable for binding.
        /// Returns <see langword="null"/> for null, empty, whitespace, or malformed URLs.
        /// </summary>
        /// <param name="posterUrl">The URL string to convert.</param>
        /// <returns>
        /// A <see cref="BitmapImage"/> if the URL is valid; otherwise <see langword="null"/>.
        /// </returns>
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
        /// Validates the selected pool size and, if valid, starts the tournament via the logic service.
        /// On success, transitions to the match view and displays the first match.
        /// On failure, sets <see cref="SetupErrorMessage"/> with a descriptive message.
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
                this.UpdateCurrentMatchDisplay();
                this.CurrentViewState = MatchViewState;
            }
            catch (Exception exception)
            {
                this.SetupErrorMessage = $"Failed to start tournament: {exception.Message}";
            }
        }

        /// <summary>
        /// Advances the tournament by recording the winning movie of the current match.
        /// If the tournament is now complete, transitions to the winner view.
        /// Otherwise, updates the display with the next match.
        /// </summary>
        /// <param name="movieId">The identifier of the movie selected as the winner of the current match.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous selection operation.</returns>
        [RelayCommand]
        public async Task SelectMovieAsync(int movieId)
        {
            await this.tournamentLogicService.AdvanceWinnerAsync(CurrentUserId, movieId);

            if (this.tournamentLogicService.IsTournamentComplete())
            {
                this.WinnerMovie = this.tournamentLogicService.GetFinalWinner();
                this.CurrentViewState = WinnerViewState;
            }
            else
            {
                this.UpdateCurrentMatchDisplay();
            }
        }

        /// <summary>
        /// Resets the tournament state via the logic service, returns to the setup view,
        /// and reloads setup data (pool size and background posters) asynchronously.
        /// </summary>
        [RelayCommand]
        public void ResetTournament()
        {
            this.tournamentLogicService.ResetTournament();
            this.CurrentViewState = SetupViewState;
            _ = this.LoadSetupDataAsync();
        }

        /// <summary>
        /// Loads the maximum pool size and up to four background poster URLs from the repository.
        /// Missing poster slots are filled with fallback images hosted on themoviedb.org.
        /// Sets <see cref="SetupErrorMessage"/> if the repository call fails.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous load operation.</returns>
        public async Task LoadSetupDataAsync()
        {
            try
            {
                await Task.Yield();

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
                this.SetupErrorMessage = $"Error loading pool data: {exception.Message}";
            }
        }

        /// <summary>
        /// Reads the current match from the logic service and updates the bound properties
        /// <see cref="MovieOptionA"/>, <see cref="MovieOptionB"/>, and <see cref="RoundDisplay"/>.
        /// Does nothing if <see cref="ITournamentLogicService.GetCurrentMatch"/> returns <see langword="null"/>.
        /// </summary>
        private void UpdateCurrentMatchDisplay()
        {
            MatchPair currentMatch = this.tournamentLogicService.GetCurrentMatch();
            if (currentMatch == null)
            {
                return;
            }

            this.MovieOptionA = currentMatch.FirstMovie;
            this.MovieOptionB = currentMatch.SecondMovie;
            this.RoundDisplay = $"Round {this.tournamentLogicService.CurrentState.CurrentRound}";
        }
    }
}

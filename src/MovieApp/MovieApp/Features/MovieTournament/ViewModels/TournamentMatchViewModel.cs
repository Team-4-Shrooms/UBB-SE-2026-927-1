using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.MovieTournament;

namespace MovieApp.Features.MovieTournament.ViewModels
{
    /// <summary>
    /// View model for the tournament match page, exposing the two competing movies
    /// and handling winner selection and round progression.
    /// </summary>
    public partial class TournamentMatchViewModel : ObservableObject
    {
        private const int CurrentUserId = 1;

        private readonly ITournamentLogicService tournamentLogicService;

        [ObservableProperty]
        private Movie? movieOptionA;

        [ObservableProperty]
        private Movie? movieOptionB;

        [ObservableProperty]
        private ImageSource? movieOptionAImage;

        [ObservableProperty]
        private ImageSource? movieOptionBImage;

        [ObservableProperty]
        private string roundDisplay = string.Empty;

        /// <summary>
        /// Raised when all matches are complete and the view should navigate to the winner page.
        /// </summary>
        public event EventHandler? TournamentComplete;

        /// <summary>
        /// Raised when the user navigates back and the view should return to the setup page.
        /// </summary>
        public event EventHandler? NavigateBack;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentMatchViewModel"/> class
        /// and immediately loads the current match.
        /// </summary>
        /// <param name="tournamentLogicService">The service managing tournament bracket logic.</param>
        public TournamentMatchViewModel(ITournamentLogicService tournamentLogicService)
        {
            this.tournamentLogicService = tournamentLogicService;
            this.RefreshCurrentMatch();
        }

        /// <summary>
        /// Refreshes the displayed match by reading the current pending match from the service.
        /// Updates both the movie models and their corresponding image sources.
        /// Does nothing if <see cref="ITournamentLogicService.GetCurrentMatch"/> returns <see langword="null"/>.
        /// </summary>
        public void RefreshCurrentMatch()
        {
            MatchPair currentMatch = this.tournamentLogicService.GetCurrentMatch();
            if (currentMatch == null)
            {
                return;
            }

            this.MovieOptionA = currentMatch.FirstMovie;
            this.MovieOptionB = currentMatch.SecondMovie;
            this.MovieOptionAImage = this.GetImageSource(currentMatch.FirstMovie.PosterUrl);
            this.MovieOptionBImage = currentMatch.SecondMovie != null
                ? this.GetImageSource(currentMatch.SecondMovie.PosterUrl)
                : null;
            this.RoundDisplay = $"Round {this.tournamentLogicService.CurrentState.CurrentRound}";
        }

        /// <summary>
        /// Converts a poster URL into an <see cref="ImageSource"/> suitable for binding.
        /// Returns <see langword="null"/> if the URL is null, empty, whitespace, or malformed.
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
        /// Records the selected movie as the winner of the current match and advances the tournament.
        /// Raises <see cref="TournamentComplete"/> if no further matches remain,
        /// otherwise refreshes the display with the next match.
        /// </summary>
        /// <param name="movieId">The identifier of the movie selected as the winner.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous selection operation.</returns>
        [RelayCommand]
        public async Task SelectMovieAsync(int movieId)
        {
            await this.tournamentLogicService.AdvanceWinnerAsync(CurrentUserId, movieId);

            if (this.tournamentLogicService.IsTournamentComplete())
            {
                this.TournamentComplete?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.RefreshCurrentMatch();
            }
        }

        /// <summary>
        /// Resets the active tournament and raises <see cref="NavigateBack"/>
        /// to return the view to the setup page.
        /// </summary>
        [RelayCommand]
        public void GoBack()
        {
            this.tournamentLogicService.ResetTournament();
            this.NavigateBack?.Invoke(this, EventArgs.Empty);
        }
    }
}

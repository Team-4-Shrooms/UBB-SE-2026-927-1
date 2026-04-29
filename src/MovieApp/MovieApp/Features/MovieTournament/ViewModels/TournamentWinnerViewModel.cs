using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using MovieApp.Logic.Models;
using MovieApp.Features.MovieTournament.Services;

namespace MovieApp.Features.MovieTournament.ViewModels
{
    /// <summary>
    /// View model for the tournament winner page, exposing the winning movie
    /// and providing the command to restart the tournament flow.
    /// </summary>
    public partial class TournamentWinnerViewModel : ObservableObject
    {
        private readonly ITournamentLogicService tournamentLogicService;

        [ObservableProperty]
        private Movie? winnerMovie;

        [ObservableProperty]
        private ImageSource? winnerMovieImage;

        /// <summary>
        /// Raised when the user chooses to start another tournament
        /// and the view should navigate back to the setup page.
        /// </summary>
        public event EventHandler? NavigateToSetup;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentWinnerViewModel"/> class
        /// and immediately loads the final winner if the tournament is complete.
        /// </summary>
        /// <param name="tournamentLogicService">The service managing tournament bracket logic.</param>
        public TournamentWinnerViewModel(ITournamentLogicService tournamentLogicService)
        {
            this.tournamentLogicService = tournamentLogicService;

            if (this.tournamentLogicService.IsTournamentComplete())
            {
                this.WinnerMovie = this.tournamentLogicService.GetFinalWinner();
                this.WinnerMovieImage = this.GetImageSource(this.WinnerMovie.PosterUrl);
            }
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
        /// Resets the active tournament and raises <see cref="NavigateToSetup"/>
        /// to return to the setup page.
        /// </summary>
        [RelayCommand]
        public void StartAnotherTournament()
        {
            this.tournamentLogicService.ResetTournament();
            this.NavigateToSetup?.Invoke(this, EventArgs.Empty);
        }
    }
}
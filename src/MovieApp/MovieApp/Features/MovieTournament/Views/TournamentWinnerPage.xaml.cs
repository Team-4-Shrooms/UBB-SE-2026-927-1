using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.Features.MovieTournament.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace MovieApp.Features.MovieTournament.Views
{
    /// <summary>
    /// Code-behind for the tournament winner page.
    /// Resolves the view model and wires up the navigation event
    /// that transitions back to the setup page when the user starts another tournament.
    /// </summary>
    public sealed partial class TournamentWinnerPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentWinnerPage"/> class,
        /// resolves the view model, and subscribes to the navigate to setup event.
        /// </summary>
        public TournamentWinnerPage()
        {
            this.ViewModel = Ioc.Default.GetRequiredService<TournamentWinnerViewModel>();
            this.InitializeComponent();

            this.ViewModel.NavigateToSetup += (_, _) =>
                this.Frame.Navigate(typeof(TournamentSetupPage));
        }

        /// <summary>
        /// Gets the view model that exposes the winning movie and the restart command.
        /// </summary>
        public TournamentWinnerViewModel ViewModel { get; }

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // this.ViewModel.Initialize();
        }
    }
}

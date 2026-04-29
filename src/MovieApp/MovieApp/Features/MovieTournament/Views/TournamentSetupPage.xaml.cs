using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.Features.MovieTournament.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace MovieApp.Features.MovieTournament.Views
{
    /// <summary>
    /// Code-behind for the tournament setup page.
    /// Resolves the view model and wires up the navigation event
    /// that transitions to the match page once the tournament starts.
    /// </summary>
    public sealed partial class TournamentSetupPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentSetupPage"/> class,
        /// resolves the view model, and subscribes to the tournament started event.
        /// </summary>
        public TournamentSetupPage()
        {
            this.ViewModel = Ioc.Default.GetRequiredService<TournamentSetupViewModel>();
            this.InitializeComponent();

            this.ViewModel.TournamentStarted += (_, _) =>
                this.Frame.Navigate(typeof(TournamentMatchPage));
        }

        /// <inheritdoc/>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // await this.ViewModel.InitializeAsync();
        }

        /// <summary>
        /// Gets the view model that drives the pool size selection and tournament startup.
        /// </summary>
        public TournamentSetupViewModel ViewModel { get; }
    }
}

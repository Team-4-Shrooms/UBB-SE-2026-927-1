using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.Features.PersonalityMatch.Models;
using MovieApp.Features.PersonalityMatch.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace MovieApp.Features.PersonalityMatch.Views
{
    /// <summary>
    /// A page that displays detailed profile and compatibility information for a specific matched user, including their top movie preferences and Facebook account.
    /// </summary>
    public sealed partial class MatchedUserDetailPage : Page
    {
        /// <summary>
        /// Gets the ViewModel that drives this page's data and logic.
        /// </summary>
        public MatchedUserDetailViewModel ViewModel { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="MatchedUserDetailPage"/> and resolves
        /// its ViewModel from the application's dependency injection container.
        /// </summary>
        public MatchedUserDetailPage()
        {
            ViewModel = Ioc.Default.GetRequiredService<MatchedUserDetailViewModel>();
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the page is navigated to. Extracts the <see cref="MatchResult"/>
        /// passed as a navigation parameter and triggers the ViewModel to load the matched user's details asynchronously.
        /// </summary>
        /// <param name="navigationEventArguments">
        /// The navigation event data, expected to carry a <see cref="MatchResult"/> as its parameter.
        /// </param>
        protected override async void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);

            bool navigationParameterIsMatchResult = navigationEventArguments.Parameter is MatchResult;
            if (navigationParameterIsMatchResult)
            {
                MatchResult matchResult = (MatchResult)navigationEventArguments.Parameter;
                await ViewModel.LoadUserDetailAsync(
                    matchResult.MatchedUserId,
                    matchResult.MatchScore,
                    matchResult.FacebookAccount,
                    matchResult.MatchedUsername,
                    matchResult.IsSelfView);
            }
        }

        /// <summary>
        /// Handles the back button click event by navigating to the previous page in the frame's back stack, if one exists.
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="routedEventArguments">The event data for the routed event.</param>
        private void BackButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            bool canNavigateBack = Frame.CanGoBack;
            if (canNavigateBack)
            {
                Frame.GoBack();
            }
        }
    }
}

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.PersonalityMatch;

namespace MovieApp.Features.PersonalityMatch.ViewModels
{
    /// <summary>
    /// ViewModel for the matched user detail view, responsible for loading and exposing profile information,
    /// compatibility score, Facebook account, and top movie preferences for a specific matched user.
    /// </summary>
    public partial class MatchedUserDetailViewModel : ObservableObject
    {
        private readonly IPersonalityMatchingService personalityMatchingService;

        private const int TopMoviePreferencesCount = 5;
        private const string DetailLoadErrorMessagePrefix = "Failed to load details: ";

        /// <summary>
        /// Gets or sets the username of the matched user.
        /// </summary>
        [ObservableProperty]
        private string matchedUsername = string.Empty;

        /// <summary>
        /// Gets or sets the compatibility score between the current user and the matched user.
        /// </summary>
        [ObservableProperty]
        private double matchScore;

        /// <summary>
        /// Gets or sets the Facebook account of the matched user.
        /// </summary>
        [ObservableProperty]
        private string facebookAccount = string.Empty;

        /// <summary>
        /// Gets or sets the full profile of the matched user.
        /// Returns <see langword="null"/> if the profile could not be loaded.
        /// </summary>
        [ObservableProperty]
        private UserProfile? userProfile;

        /// <summary>
        /// Gets or sets a value indicating whether the view is currently loading data.
        /// </summary>
        [ObservableProperty]
        private bool isLoading;

        /// <summary>
        /// Gets or sets the error message to display if data loading fails.
        /// Returns <see langword="null"/> if no error has occurred.
        /// </summary>
        [ObservableProperty]
        private string? errorMessage;

        /// <summary>
        /// Gets or sets a value indicating whether a valid user profile was successfully loaded.
        /// </summary>
        [ObservableProperty]
        private bool hasProfile;

        /// <summary>
        /// Gets or sets a value indicating whether the compatibility score section should be visible.
        /// Set to <see langword="false"/> when the user is viewing their own profile.
        /// </summary>
        [ObservableProperty]
        private bool showCompatibility = true;

        /// <summary>
        /// Gets the collection of top-rated movie preferences for the matched user,
        /// enriched with movie titles and best movie flags.
        /// </summary>
        public ObservableCollection<MoviePreferenceDisplay> TopPreferences { get; } = new ();

        /// <summary>
        /// Initializes a new instance of <see cref="MatchedUserDetailViewModel"/> with the specified matching service.
        /// </summary>
        /// <param name="personalityMatchingService">The service used to retrieve user profile and preference data.</param>
        public MatchedUserDetailViewModel(IPersonalityMatchingService personalityMatchingService)
        {
            this.personalityMatchingService = personalityMatchingService;
        }

        /// <summary>
        /// Asynchronously loads the detail information for the specified matched user,
        /// including their username, profile, Facebook account, match score, and top movie preferences.
        /// </summary>
        /// <param name="userId">The identifier of the user whose details are to be loaded.</param>
        /// <param name="matchScore">The compatibility score between the current user and the matched user.</param>
        /// <param name="facebookAccount">The Facebook account of the matched user.</param>
        /// <param name="username">
        /// An optional pre-resolved username. If empty or <see langword="null"/>, the username will be fetched from the service.
        /// </param>
        /// <param name="isSelfView">
        /// Indicates whether the current user is viewing their own profile. When <see langword="true"/>, the compatibility score section is hidden.
        /// </param>
        public async Task LoadUserDetailAsync(int userId, double matchScore, string facebookAccount, string username = "", bool isSelfView = false)
        {
            ResetViewState(isSelfView);

            MatchedUsername = await ResolveUsernameAsync(userId, username);
            MatchScore = matchScore;
            FacebookAccount = facebookAccount;

            try
            {
                await LoadUserProfileAsync(userId);
                await LoadTopMoviePreferencesAsync(userId);
            }
            catch (Exception exception)
            {
                ErrorMessage = DetailLoadErrorMessagePrefix + exception.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Resets all view state properties to their default values in preparation for a new data load.
        /// Sets <see cref="showCompatibility"/> based on whether the view is a self-view.
        /// </summary>
        /// <param name="isSelfView">
        /// Indicates whether the current user is viewing their own profile. When <see langword="true"/>, compatibility is hidden.
        /// </param>
        private void ResetViewState(bool isSelfView)
        {
            IsLoading = true;
            ErrorMessage = null;
            HasProfile = false;
            ShowCompatibility = !isSelfView;
            TopPreferences.Clear();
        }

        /// <summary>
        /// Resolves the display username for the specified user. Returns the provided username directly
        /// if it is non-empty; otherwise fetches it from the service.
        /// </summary>
        /// <param name="userId">The identifier of the user whose username is to be resolved.</param>
        /// <param name="username">An optional pre-resolved username.</param>
        /// <returns>
        /// The provided username if non-empty; otherwise the username retrieved from the service.
        /// </returns>
        private async Task<string> ResolveUsernameAsync(int userId, string username)
        {
            bool usernameWasProvided = !string.IsNullOrEmpty(username);
            return usernameWasProvided
                ? username
                : await this.personalityMatchingService.GetUsernameAsync(userId);
        }

        /// <summary>
        /// Asynchronously loads the profile for the specified user from the service
        /// and updates <see cref="userProfile"/> and <see cref="hasProfile"/> accordingly.
        /// If no profile is found, <see cref="hasProfile"/> remains <see langword="false"/>.
        /// </summary>
        /// <param name="userId">The identifier of the user whose profile is to be loaded.</param>
        private async Task LoadUserProfileAsync(int userId)
        {
            UserProfile? profile = await this.personalityMatchingService.GetUserProfileAsync(userId);
            bool profileWasFound = profile != null;
            if (profileWasFound)
            {
                UserProfile = profile;
                HasProfile = true;
            }
        }

        /// <summary>
        /// Asynchronously loads the top-rated movie preferences for the specified user and populates the <see cref="TopPreferences"/> collection.
        /// </summary>
        /// <param name="userId">The identifier of the user whose top preferences are to be loaded.</param>
        private async Task LoadTopMoviePreferencesAsync(int userId)
        {
            List<MoviePreferenceDisplay> topMoviePreferences =
                await this.personalityMatchingService.GetTopMoviePreferencesAsync(userId, TopMoviePreferencesCount);

            foreach (MoviePreferenceDisplay moviePreference in topMoviePreferences)
            {
                TopPreferences.Add(moviePreference);
            }
        }
    }
}

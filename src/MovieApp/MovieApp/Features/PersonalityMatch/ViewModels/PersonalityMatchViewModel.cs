using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.Features.PersonalityMatch.Models;
using MovieApp.Features.PersonalityMatch.Services;

namespace MovieApp.Features.PersonalityMatch.ViewModels
{
    /// <summary>
    /// ViewModel for the Personality Match list page.
    /// Handles loading matches, fallback to random users, user selection, and the account switcher panel.
    /// Owner: Madi
    /// </summary>
    public partial class PersonalityMatchViewModel : ObservableObject
    {
        private readonly IPersonalityMatchingService personalityMatchingService;

        private const int MaximumMatchCount = 10;
        private const string InitialStatusMessage = "Discover users with similar taste.";
        private const string ComputingMatchesStatusMessage = "Computing matches…";
        private const string SingleMatchStatusMessage = "Found 1 match!";
        private const string MultipleMatchesStatusMessageFormat = "Found {0} matches!";
        private const string ErrorStatusMessagePrefix = "Error: ";
        private const string DefaultUsername = "Alex Carter";
        private const string DefaultFacebookAccount = "fb_alex_carter";
        private const int DefaultActiveUserId = 1;
        private const string CurrentPageTitle = "Personality Match";

        /// <summary>
        /// A static dictionary of demo accounts used for development and testing,
        /// mapping each user identifier to a username and Facebook account pair.
        /// </summary>
        private static readonly Dictionary<int, (string Username, string FacebookAccount)> DemoAccounts = new ()
        {
            [1] = ("Alex Carter", "fb_alex_carter"),
            [2] = ("Alice Rivers", "fb_alice_rivers"),
            [3] = ("Bob Chen", "fb_bob_chen"),
            [4] = ("Carol Hayes", "fb_carol_hayes"),
            [9] = ("Sam Taylor", "fb_sam_taylor"),
        };

        /// <summary>
        /// The identifier of the currently active (logged-in) user.
        /// </summary>
        private int activeUserId = DefaultActiveUserId;

        /// <summary>
        /// The list of user identifiers currently logged into the account switcher.
        /// </summary>
        private readonly List<int> loggedAccountIds = new () { 1, 2, 9 };

        /// <summary>
        /// Gets or sets the title displayed at the top of the personality match page.
        /// </summary>
        [ObservableProperty]
        private string pageTitle = CurrentPageTitle;

        /// <summary>
        /// Gets or sets the status message shown to the user reflecting the current state of the page,
        /// such as loading progress, match count, or error information.
        /// </summary>
        [ObservableProperty]
        private string statusMessage = InitialStatusMessage;

        /// <summary>
        /// Gets or sets a value indicating whether the page is currently loading data.
        /// </summary>
        [ObservableProperty]
        private bool isLoading;

        /// <summary>
        /// Gets or sets a value indicating whether the no-matches state should be displayed,
        /// triggering the fallback random users section to appear.
        /// </summary>
        [ObservableProperty]
        private bool showNoMatch;

        /// <summary>
        /// Gets or sets a value indicating whether at least one personality match was found.
        /// </summary>
        [ObservableProperty]
        private bool hasMatches;

        /// <summary>
        /// Gets or sets a value indicating whether the account switcher panel is currently open.
        /// </summary>
        [ObservableProperty]
        private bool isAccountPanelOpen;

        /// <summary>
        /// Gets or sets the username of the currently active user.
        /// </summary>
        [ObservableProperty]
        private string currentUsername = DefaultUsername;

        /// <summary>
        /// Gets or sets the Facebook account of the currently active user.
        /// </summary>
        [ObservableProperty]
        private string currentFacebookAccount = DefaultFacebookAccount;

        /// <summary>
        /// Gets the collection of personality match results for the current user. Populated when at least one match is found.
        /// </summary>
        public ObservableCollection<MatchResult> MatchResults { get; } = new ();

        /// <summary>
        /// Gets the collection of randomly selected users shown as a fallback when no personality matches are found.
        /// </summary>
        public ObservableCollection<MatchResult> FallbackUsers { get; } = new ();

        /// <summary>
        /// Gets the collection of other logged-in accounts available in the account switcher, excluding the currently active user.
        /// </summary>
        public ObservableCollection<UserAccountModel> OtherAccounts { get; } = new ();

        /// <summary>
        /// Raised when the user selects a match result and navigation to the detail view is requested.
        /// </summary>
        public event Action<MatchResult>? NavigateToDetail;

        /// <summary>
        /// Raised when the user selects their own account in the switcher panel and navigation to their own detail view is requested.
        /// </summary>
        public event Action<UserAccountModel>? NavigateToCurrentUserDetail;

        /// <summary>
        /// Initializes a new instance of <see cref="PersonalityMatchViewModel"/> with the specified matching service.
        /// </summary>
        /// <param name="personalityMatchingService">The service used to retrieve personality matches and random users.</param>
        public PersonalityMatchViewModel(IPersonalityMatchingService personalityMatchingService)
        {
            this.personalityMatchingService = personalityMatchingService;
            RefreshAccountCollections();
        }

        /// <summary>
        /// Asynchronously loads personality matches for the active user.
        /// If no matches are found, falls back to loading a random selection of users.
        /// Updates all relevant observable state including match results, status message, and loading flags.
        /// </summary>
        [RelayCommand]
        public async Task LoadMatchesAsync()
        {
            ResetMatchState();

            try
            {
                List<MatchResult> topMatches = await this.personalityMatchingService.GetTopMatchesAsync(activeUserId, MaximumMatchCount);

                bool matchesWereFound = topMatches.Count > 0;
                if (matchesWereFound)
                {
                    PopulateMatchResults(topMatches);
                }
                else
                {
                    await PopulateFallbackUsersAsync();
                }
            }
            catch (Exception exception)
            {
                StatusMessage = ErrorStatusMessagePrefix + exception.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Navigates to the detail view for the specified match result.
        /// Does nothing if <paramref name="match"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="match">The match result selected by the user.</param>
        [RelayCommand]
        private void ViewUserDetail(MatchResult? match)
        {
            bool matchIsValid = match != null;
            if (matchIsValid)
            {
                NavigateToDetail?.Invoke(match!);
            }
        }

        /// <summary>
        /// Toggles the visibility of the account switcher panel.
        /// </summary>
        [RelayCommand]
        private void ToggleAccountPanel()
        {
            IsAccountPanelOpen = !IsAccountPanelOpen;
        }

        /// <summary>
        /// Switches the active user to the specified account, closes the account panel,
        /// and reloads matches for the newly active user.
        /// Does nothing if <paramref name="account"/> is <see langword="null"/> or already active.
        /// </summary>
        /// <param name="account">The account to switch to.</param>
        [RelayCommand]
        private async Task SwitchAccount(UserAccountModel? account)
        {
            bool accountIsInvalid = account == null;
            bool accountIsAlreadyActive = account?.UserId == activeUserId;
            if (accountIsInvalid || accountIsAlreadyActive)
            {
                return;
            }

            activeUserId = account!.UserId;
            CurrentUsername = account.Username;
            CurrentFacebookAccount = account.FacebookAccount;
            IsAccountPanelOpen = false;

            RefreshAccountCollections();
            await LoadMatchesAsync();
        }

        /// <summary>
        /// Closes the account panel and raises the <see cref="NavigateToCurrentUserDetail"/> event
        /// to navigate to the detail view of the currently active user.
        /// </summary>
        [RelayCommand]
        private void ViewCurrentAccountDetail()
        {
            IsAccountPanelOpen = false;
            UserAccountModel currentUserAccount = BuildCurrentUserAccountModel();
            NavigateToCurrentUserDetail?.Invoke(currentUserAccount);
        }

        /// <summary>
        /// Adds an account to the logged-in list and refreshes the account switcher.
        /// If the account is already present, no action is taken.
        /// </summary>
        /// <param name="account">The account to add to the logged-in list.</param>
        public void AddAccount(UserAccountModel account)
        {
            bool accountIsNotYetLogged = !loggedAccountIds.Contains(account.UserId);
            if (accountIsNotYetLogged)
            {
                loggedAccountIds.Add(account.UserId);
                RefreshAccountCollections();
            }
        }

        /// <summary>
        /// Returns all demo accounts that have not yet been added to the account switcher.
        /// </summary>
        /// <returns>
        /// A read-only list of <see cref="UserAccountModel"/> records representing accounts available to be added to the switcher.
        /// </returns>
        public IReadOnlyList<UserAccountModel> GetAvailableAccountsToAdd()
        {
            List<UserAccountModel> availableAccounts = new List<UserAccountModel>();
            foreach (KeyValuePair<int, (string Username, string FacebookAccount)> demoAccountEntry in DemoAccounts)
            {
                bool accountIsNotLogged = !loggedAccountIds.Contains(demoAccountEntry.Key);
                if (accountIsNotLogged)
                {
                    availableAccounts.Add(new UserAccountModel
                    {
                        UserId = demoAccountEntry.Key,
                        Username = demoAccountEntry.Value.Username,
                        FacebookAccount = demoAccountEntry.Value.FacebookAccount,
                    });
                }
            }
            return availableAccounts;
        }

        /// <summary>
        /// Resets all match-related state properties to their default values in preparation for a new match load operation.
        /// </summary>
        private void ResetMatchState()
        {
            IsLoading = true;
            ShowNoMatch = false;
            HasMatches = false;
            MatchResults.Clear();
            FallbackUsers.Clear();
            StatusMessage = ComputingMatchesStatusMessage;
        }

        /// <summary>
        /// Populates the <see cref="MatchResults"/> collection with the provided matches and updates the status message and <see cref="hasMatches"/> flag accordingly.
        /// </summary>
        /// <param name="matches">The list of match results to display.</param>
        private void PopulateMatchResults(List<MatchResult> matches)
        {
            foreach (MatchResult match in matches)
            {
                MatchResults.Add(match);
            }

            HasMatches = true;
            bool isSingleMatch = matches.Count == 1;
            StatusMessage = isSingleMatch
                ? SingleMatchStatusMessage
                : string.Format(MultipleMatchesStatusMessageFormat, matches.Count);
        }

        /// <summary>
        /// Asynchronously loads a random selection of users as a fallback and populates the <see cref="FallbackUsers"/> collection. Sets the no-match display state.
        /// </summary>
        private async Task PopulateFallbackUsersAsync()
        {
            ShowNoMatch = true;
            StatusMessage = string.Empty;

            List<MatchResult> randomUsers = await this.personalityMatchingService.GetRandomUsersAsync(activeUserId, MaximumMatchCount);
            foreach (MatchResult randomUser in randomUsers)
            {
                FallbackUsers.Add(randomUser);
            }
        }

        /// <summary>
        /// Constructs a <see cref="UserAccountModel"/> representing the currently active user
        /// using the current username, Facebook account, and active user identifier.
        /// </summary>
        /// <returns>
        /// A <see cref="UserAccountModel"/> populated with the active user's details.
        /// </returns>
        private UserAccountModel BuildCurrentUserAccountModel()
        {
            return new UserAccountModel
            {
                UserId = activeUserId,
                Username = CurrentUsername,
                FacebookAccount = CurrentFacebookAccount,
            };
        }

        /// <summary>
        /// Rebuilds the <see cref="OtherAccounts"/> collection to reflect the current logged-in accounts, excluding the currently active user.
        /// </summary>
        private void RefreshAccountCollections()
        {
            OtherAccounts.Clear();
            foreach (int loggedUserId in loggedAccountIds)
            {
                bool isActiveUser = loggedUserId == activeUserId;
                bool isNotInDemoAccounts = !DemoAccounts.TryGetValue(loggedUserId, out (string Username, string FacebookAccount) accountInfo);
                if (isActiveUser || isNotInDemoAccounts)
                {
                    continue;
                }

                OtherAccounts.Add(new UserAccountModel
                {
                    UserId = loggedUserId,
                    Username = accountInfo.Username,
                    FacebookAccount = accountInfo.FacebookAccount,
                });
            }
        }

        /// <summary>
        /// Constructs a synthetic <see cref="MatchResult"/> representing the current user's own profile,
        /// with a perfect match score and <see cref="MatchResult.IsSelfView"/> set to <see langword="true"/>
        /// to hide the compatibility bar on the detail page.
        /// </summary>
        /// <param name="account">The account model of the currently active user.</param>
        /// <returns>
        /// A <see cref="MatchResult"/> populated with the account's details, a match score of 100, and <see cref="MatchResult.IsSelfView"/> set to <see langword="true"/>.
        /// </returns>
        public MatchResult BuildSelfViewMatchResult(UserAccountModel account)
        {
            const double PerfectMatchScore = 100;
            return new MatchResult
            {
                MatchedUserId = account.UserId,
                MatchedUsername = account.Username,
                MatchScore = PerfectMatchScore,
                FacebookAccount = account.FacebookAccount,
                IsSelfView = true,
            };
        }
    }
}

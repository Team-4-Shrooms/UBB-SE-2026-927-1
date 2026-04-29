using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MovieApp.Features.PersonalityMatch.Models;
using MovieApp.Features.PersonalityMatch.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace MovieApp.Features.PersonalityMatch.Views
{
    /// <summary>
    /// A page that displays the personality match list for the currently active user,
    /// including top compatibility matches, fallback random users, and an account switcher panel.
    /// </summary>`n    public sealed partial class PersonalityMatchPage : Page
    {
        private const double AccountPickerListViewHeight = 200;
        private const string AccountPickerItemTemplateKey = "AccountPickerItemTemplate";
        private const string AddAccountDialogTitle = "Add account";
        private const string AddAccountDialogPrimaryButtonText = "Add";
        private const string AddAccountDialogCancelButtonText = "Cancel";
        private const string NoAccountsAvailableMessage = "All available accounts have already been added.";
        private const string NoAccountsAvailableCloseButtonText = "OK";

        /// <summary>
        /// Gets the ViewModel that drives this page's data and logic.
        /// </summary>
        public PersonalityMatchViewModel ViewModel { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="PersonalityMatchPage"/>, resolves its ViewModel
        /// from the dependency injection container, and subscribes to navigation events.
        /// Subscriptions are cleaned up when the page is unloaded.
        /// </summary>
        public PersonalityMatchPage()
        {
            ViewModel = Ioc.Default.GetRequiredService<PersonalityMatchViewModel>();
            this.InitializeComponent();

            ViewModel.NavigateToDetail += OnNavigateToDetail;
            ViewModel.NavigateToCurrentUserDetail += OnNavigateToCurrentUserDetail;
            this.Unloaded += OnPageUnloaded;
        }

        /// <summary>
        /// Handles the page loaded event. Triggers an initial match load if no results are currently present and the no-match state has not already been set.
        /// </summary>
        /// <param name="sender">The source of the loaded event.</param>
        /// <param name="routedEventArguments">The event data.</param>
        private async void Page_Loaded(object sender, RoutedEventArgs routedEventArguments)
        {
            bool hasNoResultsYet = ViewModel.MatchResults.Count == 0 && !ViewModel.ShowNoMatch;
            if (hasNoResultsYet)
            {
                await ViewModel.LoadMatchesAsync();
            }
        }

        /// <summary>
        /// Unsubscribes from ViewModel navigation events when the page is unloaded to prevent memory leaks.
        /// </summary>
        /// <param name="sender">The source of the unloaded event.</param>
        /// <param name="routedEventArguments">The event data.</param>
        private void OnPageUnloaded(object sender, RoutedEventArgs routedEventArguments)
        {
            ViewModel.NavigateToDetail -= OnNavigateToDetail;
            ViewModel.NavigateToCurrentUserDetail -= OnNavigateToCurrentUserDetail;
        }

        /// <summary>
        /// Handles item clicks in the match results list view by forwarding the selected <see cref="MatchResult"/> to the ViewModel's view user detail command.
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="itemClickEventArguments">The event data containing the clicked item.</param>
        private void MatchListView_ItemClick(object sender, ItemClickEventArgs itemClickEventArguments)
        {
            bool clickedItemIsMatchResult = itemClickEventArguments.ClickedItem is MatchResult;
            if (clickedItemIsMatchResult)
            {
                MatchResult matchResult = (MatchResult)itemClickEventArguments.ClickedItem;
                ViewModel.ViewUserDetailCommand.Execute(matchResult);
            }
        }

        /// <summary>
        /// Handles item clicks in the fallback random users list view by forwarding the selected <see cref="MatchResult"/> to the ViewModel's view user detail command.
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="itemClickEventArguments">The event data containing the clicked item.</param>
        private void FallbackListView_ItemClick(object sender, ItemClickEventArgs itemClickEventArguments)
        {
            bool clickedItemIsMatchResult = itemClickEventArguments.ClickedItem is MatchResult;
            if (clickedItemIsMatchResult)
            {
                MatchResult matchResult = (MatchResult)itemClickEventArguments.ClickedItem;
                ViewModel.ViewUserDetailCommand.Execute(matchResult);
            }
        }

        /// <summary>
        /// Handles item clicks in the account switcher list by forwarding the selected
        /// <see cref="UserAccountModel"/> to the ViewModel's switch account command.
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="itemClickEventArguments">The event data containing the clicked item.</param>
        private void OtherAccountsList_ItemClick(object sender, ItemClickEventArgs itemClickEventArguments)
        {
            bool clickedItemIsUserAccount = itemClickEventArguments.ClickedItem is UserAccountModel;
            if (clickedItemIsUserAccount)
            {
                UserAccountModel userAccount = (UserAccountModel)itemClickEventArguments.ClickedItem;
                ViewModel.SwitchAccountCommand.Execute(userAccount);
            }
        }

        /// <summary>
        /// Handles the add account button click. Presents a dialog listing all accounts
        /// not yet added to the switcher, and adds the selected account via the ViewModel
        /// if the user confirms. Shows an informational dialog if no accounts are available to add.
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="routedEventArguments">The event data.</param>
        private async void AddAccount_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            IReadOnlyList<UserAccountModel> availableAccounts = ViewModel.GetAvailableAccountsToAdd();

            bool noAccountsAreAvailable = availableAccounts.Count == 0;
            if (noAccountsAreAvailable)
            {
                await ShowNoAccountsAvailableDialogAsync();
                return;
            }

            ListView accountPickerListView = BuildAccountPickerListView(availableAccounts);
            ContentDialog addAccountDialog = BuildAddAccountDialog(accountPickerListView);

            ContentDialogResult dialogResult = await addAccountDialog.ShowAsync();

            bool userConfirmedSelection = dialogResult == ContentDialogResult.Primary;
            bool anAccountWasSelected = accountPickerListView.SelectedItem is UserAccountModel;
            if (userConfirmedSelection && anAccountWasSelected)
            {
                UserAccountModel selectedAccount = (UserAccountModel)accountPickerListView.SelectedItem;
                ViewModel.AddAccount(selectedAccount);
            }
        }

        /// <summary>
        /// Handles the <see cref="PersonalityMatchViewModel.NavigateToDetail"/> event
        /// by navigating to <see cref="MatchedUserDetailPage"/> with the selected match as the parameter.
        /// </summary>
        /// <param name="matchResult">The match result to display in the detail page.</param>
        private void OnNavigateToDetail(MatchResult matchResult)
        {
            Frame.Navigate(typeof(MatchedUserDetailPage), matchResult);
        }

        /// <summary>
        /// Handles the <see cref="PersonalityMatchViewModel.NavigateToCurrentUserDetail"/> event
        /// by navigating to <see cref="MatchedUserDetailPage"/> for the current user's own profile.
        /// Delegates construction of the self-view match result to the ViewModel.
        /// </summary>
        /// <param name="account">The current user's account model.</param>
        private void OnNavigateToCurrentUserDetail(UserAccountModel account)
        {
            MatchResult selfViewMatchResult = ViewModel.BuildSelfViewMatchResult(account);
            Frame.Navigate(typeof(MatchedUserDetailPage), selfViewMatchResult);
        }

        /// <summary>
        /// Displays an informational <see cref="ContentDialog"/> notifying the user
        /// that no additional accounts are available to add to the switcher.
        /// </summary>
        private async Task ShowNoAccountsAvailableDialogAsync()
        {
            ContentDialog noAccountsDialog = new ContentDialog
            {
                Title = AddAccountDialogTitle,
                Content = NoAccountsAvailableMessage,
                CloseButtonText = NoAccountsAvailableCloseButtonText,
                XamlRoot = this.XamlRoot,
            };
            await noAccountsDialog.ShowAsync();
        }

        /// <summary>
        /// Constructs a <see cref="ListView"/> populated with the provided accounts
        /// and configured with the account picker item template and single-selection mode.
        /// </summary>
        /// <param name="availableAccounts">The list of accounts to display in the picker.</param>
        /// <returns>
        /// A configured <see cref="ListView"/> ready to be embedded in the add account dialog.
        /// </returns>
        private ListView BuildAccountPickerListView(IReadOnlyList<UserAccountModel> availableAccounts)
        {
            return new ListView
            {
                ItemsSource = availableAccounts,
                SelectionMode = ListViewSelectionMode.Single,
                Height = AccountPickerListViewHeight,
                ItemTemplate = (DataTemplate)Resources[AccountPickerItemTemplateKey],
            };
        }

        /// <summary>
        /// Constructs a <see cref="ContentDialog"/> for adding an account,
        /// embedding the provided account picker list view as its content.
        /// </summary>
        /// <param name="accountPickerListView">The list view to embed as the dialog's content.</param>
        /// <returns>
        /// A configured <see cref="ContentDialog"/> ready to be shown to the user.
        /// </returns>
        private ContentDialog BuildAddAccountDialog(ListView accountPickerListView)
        {
            return new ContentDialog
            {
                Title = AddAccountDialogTitle,
                Content = accountPickerListView,
                PrimaryButtonText = AddAccountDialogPrimaryButtonText,
                CloseButtonText = AddAccountDialogCancelButtonText,
                XamlRoot = this.XamlRoot,
            };
        }
    }
}

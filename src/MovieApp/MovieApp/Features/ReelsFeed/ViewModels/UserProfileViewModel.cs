using CommunityToolkit.Mvvm.ComponentModel;
using MovieApp;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.ReelsFeed;

namespace MovieApp.Features.ReelsFeed.ViewModels
{
    /// <summary>
    /// Exposes the user's engagement metrics from <see cref="IEngagementProfileService"/>.
    /// Read by Madi's PersonalityMatch feature for matched user details.
    /// Owner: Tudor.
    /// </summary>
    public partial class UserProfileViewModel : ObservableObject
    {
        private readonly IEngagementProfileService profileService;

        [ObservableProperty]
        private UserProfile? profile;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileViewModel"/> class.
        /// </summary>
        /// <param name="profileService">Service used to load and refresh user engagement profiles.</param>
        public UserProfileViewModel(IEngagementProfileService profileService)
        {
            this.profileService = profileService;
        }

        /// <summary>
        /// Loads the engagement profile for the given user.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile should be loaded.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task LoadProfileAsync(int userId)
        {
            this.IsLoading = true;
            this.ErrorMessage = null;

            try
            {
                await this.profileService.RefreshProfileAsync(userId);
                this.Profile = await this.profileService.GetProfileAsync(userId);
            }
            catch (System.Exception ex)
            {
                this.ErrorMessage = string.Format("User Profile", ex.Message);
            }
            finally
            {
                this.IsLoading = false;
            }
        }
    }
}

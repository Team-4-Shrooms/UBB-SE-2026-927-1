using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Features.MovieSwipe.Services;

namespace MovieApp.Features.MovieSwipe.ViewModels
{
    /// <summary>
    /// ViewModel for the Movie Swipe screen. Handles the logic for the card queue and user interactions.
    /// </summary>
    public partial class MovieSwipeViewModel : ObservableObject
    {
        /// <summary> Number of cards to fetch per request. </summary>
        private const int BufferSize = 5;

        /// <summary> Threshold of remaining cards that triggers a refill. </summary>
        private const int RefillThreshold = 2;

        /// <summary> Placeholder user ID until authentication is fully integrated. </summary>
        private const int DefaultUserId = 1;

        /// <summary> The service used for swipe actions. </summary>
        private readonly ISwipeService swipeService;

        private readonly IMovieCardFeedService feedService;

        /// <summary> The service used for movie feed retrieval. </summary>
        private readonly IMovieCardFeedService movieCardFeedService;

        /// <summary> Flag to prevent concurrent refill operations. </summary>
        private bool isRefilling;

        /// <summary> Initializes a new instance of the <see cref="MovieSwipeViewModel"/> class. </summary>
        /// <param name="swipeService">The swipe action service.</param>
        /// <param name="movieCardFeedService">The feed retrieval service.</param>
        public MovieSwipeViewModel(ISwipeService swipeService, IMovieCardFeedService movieCardFeedService)
        {
            this.swipeService = swipeService;
            this.movieCardFeedService = movieCardFeedService;
            CardQueue = new ObservableCollection<Movie>();
        }

        /// <summary> The current movie card displayed to the user. </summary>
        [ObservableProperty]
        private Movie? currentCard;

        /// <summary> The collection of upcoming movie cards. </summary>
        public ObservableCollection<Movie> CardQueue { get; }

        /// <summary> Indicates whether a data loading operation is in progress. </summary>
        [ObservableProperty]
        private bool isLoading;

        /// <summary> Indicates whether there are no more movies left to swipe. </summary>
        [ObservableProperty]
        private bool isAllCaughtUp;

        /// <summary> Current status or error message for the UI. </summary>
        [ObservableProperty]
        private string statusMessage = "Swipe right to like, left to skip.";

        /// <summary> Command used to trigger the initial loading of the movie feed. </summary>
        /// <returns>A task representing the operation.</returns>
        [RelayCommand]
        public async Task InitializeAsync()
        {
            await LoadInitialCardsAsync();
        }

        /// <summary> Handles the primary logic for loading the first set of cards into the queue. </summary>
        private async Task LoadInitialCardsAsync()
        {
            try
            {
                IsLoading = true;
                IsAllCaughtUp = false;

                List<Movie> movies = await movieCardFeedService.FetchMovieFeedAsync(DefaultUserId, BufferSize);

                CardQueue.Clear();
                foreach (Movie movie in movies)
                {
                    CardQueue.Add(movie);
                }

                AdvanceToNextCard();

                const int emptyCount = 0;
                if (CardQueue.Count == emptyCount && CurrentCard == null)
                {
                    IsAllCaughtUp = true;
                    StatusMessage = "No movies found in database.";
                }
            }
            catch (Exception exception)
            {
                StatusMessage = $"Error loading movies: {exception.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary> Logic for handling a "Like" action. </summary>
        /// <returns>A task representing the operation.</returns>
        [RelayCommand]
        private async Task SwipeRightAsync()
        {
            await ProcessSwipeAsync(isLiked: true);
        }

        /// <summary> Logic for handling a "Skip" action. </summary>
        /// <returns>A task representing the operation.</returns>
        [RelayCommand]
        private async Task SwipeLeftAsync()
        {
            await ProcessSwipeAsync(isLiked: false);
        }

        /// <summary> Orchestrates the swipe by updating the UI immediately and starting persistence. </summary>
        /// <param name="isLiked">The direction of the swipe.</param>
        private async Task ProcessSwipeAsync(bool isLiked)
        {
            if (CurrentCard == null)
            {
                return;
            }

            Movie swipedCard = CurrentCard;

            // Advance immediately for responsive UI
            AdvanceToNextCard();

            try
            {
                await swipeService.UpdatePreferenceScoreAsync(DefaultUserId, swipedCard.Id, isLiked);
                await TryRefillQueueAsync(swipedCard.Id);
            }
            catch (Exception exception)
            {
                StatusMessage = $"Failed to save preference: {exception.Message}";
            }
        }

        /// <summary> Pops the next card from the queue and updates the current display. </summary>
        private void AdvanceToNextCard()
        {
            const int firstIndex = 0;
            if (CardQueue.Count > firstIndex)
            {
                CurrentCard = CardQueue[firstIndex];
                CardQueue.RemoveAt(firstIndex);
                IsAllCaughtUp = false;
            }
            else
            {
                CurrentCard = null;
                IsAllCaughtUp = true;
                StatusMessage = "No movies found in database.";
            }
        }

        /// <summary> Fetches more movies if the internal queue falls below the threshold. </summary>
        /// <param name="recentlySwipedMovieId">Optional ID to ensure the swiped movie isn't immediately re-added.</param>
        private async Task TryRefillQueueAsync(int? recentlySwipedMovieId = null)
        {
            if (isRefilling || CardQueue.Count > RefillThreshold)
            {
                return;
            }

            isRefilling = true;

            try
            {
                List<Movie> newMovies = await movieCardFeedService.FetchMovieFeedAsync(DefaultUserId, BufferSize);

                HashSet<int> existingIds = new HashSet<int>(CardQueue.Select(movie => movie.Id));
                if (CurrentCard != null)
                {
                    existingIds.Add(CurrentCard.Id);
                }

                foreach (Movie movie in newMovies)
                {
                    if (recentlySwipedMovieId.HasValue && movie.Id == recentlySwipedMovieId.Value)
                    {
                        continue;
                    }

                    if (existingIds.Contains(movie.Id))
                    {
                        continue;
                    }

                    CardQueue.Add(movie);
                    existingIds.Add(movie.Id);
                }

                const int emptyCount = 0;
                if (CurrentCard == null && CardQueue.Count > emptyCount)
                {
                    IsAllCaughtUp = false;
                    StatusMessage = "Swipe right to like, left to skip.";
                    AdvanceToNextCard();
                }
            }
            catch (Exception exception)
            {
                StatusMessage = $"Sync Error: {exception.Message}";
            }
            finally
            {
                isRefilling = false;
            }
        }
    }
}

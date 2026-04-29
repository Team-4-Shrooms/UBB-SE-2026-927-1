namespace MovieApp.Features.TrailerScraping.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using MovieApp.DataLayer.Models;
    using MovieApp.Features.TrailerScraping.Services;

    /// <summary>
    /// ViewModel for the Trailer Scraping admin dashboard.
    /// Handles movie autocomplete, scrape execution, and log display.
    /// Owner: Andrei.
    /// </summary>
    public partial class TrailerScrapingViewModel : ObservableObject
    {
        private const int DefaultMaxResults = 5;
        private const int MinimumSearchQueryLength = 2;
        private const int EmptyCollectionCount = 0;
        private const int TopLogEntryIndex = 0;

        private const string StatusIdle = "Idle";
        private const string StatusScraping = "Scraping...";

        private readonly IVideoIngestionService ingestionService;
        private readonly IScrapeRepository repository;

        [ObservableProperty]
        private int totalMovies;

        [ObservableProperty]
        private int totalReels;

        [ObservableProperty]
        private int totalJobs;

        [ObservableProperty]
        private int runningJobs;

        [ObservableProperty]
        private int completedJobs;

        [ObservableProperty]
        private int failedJobs;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private Movie? selectedMovie;

        [ObservableProperty]
        private bool noMovieFound;

        [ObservableProperty]
        private int maxResults = DefaultMaxResults;

        [ObservableProperty]
        private bool isScraping;

        [ObservableProperty]
        private string statusText = StatusIdle;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrailerScrapingViewModel"/> class.
        /// </summary>
        /// <param name="ingestionService">The video ingestion service.</param>
        /// <param name="repository">The scrape job repository.</param>
        public TrailerScrapingViewModel(
            IVideoIngestionService ingestionService,
            IScrapeRepository repository)
        {
            this.ingestionService = ingestionService;
            this.repository = repository;
        }

        /// <summary>
        /// Gets the collection of movies suggested by the search query.
        /// </summary>
        public ObservableCollection<Movie> SuggestedMovies { get; } = new ();

        /// <summary>
        /// Gets the available options for the maximum number of search results.
        /// </summary>
        public List<int> MaxResultsOptions { get; } = new () { 5, 10, 15, 25, 50 };

        /// <summary>
        /// Gets the collection of scrape job logs.
        /// </summary>
        public ObservableCollection<ScrapeJobLog> LogEntries { get; } = new ();

        /// <summary>
        /// Gets the collection of all movies for the data table.
        /// </summary>
        public ObservableCollection<Movie> MovieTableItems { get; } = new ();

        /// <summary>
        /// Gets the collection of all reels for the data table.
        /// </summary>
        public ObservableCollection<Reel> ReelTableItems { get; } = new ();

        /// <summary>
        /// Called when the user picks a movie from the dropdown.
        /// </summary>
        /// <param name="movie">The movie selected by the user.</param>
        public void SelectMovie(Movie movie)
        {
            this.SelectedMovie = movie;
            this.SearchText = movie.Title;
            this.NoMovieFound = false;
            this.StartScrapeCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Called by the Page when it loads to populate initial data.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InitializeAsync()
        {
            await this.RefreshAsync();
        }

        /// <summary>
        /// Called when the user types in the AutoSuggestBox.
        /// Queries the Movie table for case-insensitive matches.
        /// </summary>
        /// <param name="query">The search query text.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task SearchMoviesAsync(string query)
        {
            this.SearchText = query;
            this.SelectedMovie = null;
            this.NoMovieFound = false;

            if (string.IsNullOrWhiteSpace(query) || query.Length < MinimumSearchQueryLength)
            {
                this.SuggestedMovies.Clear();
                return;
            }

            try
            {
                IList<Movie> matches = await this.repository.SearchMoviesByNameAsync(query);
                this.SuggestedMovies.Clear();

                foreach (Movie movieMatch in matches)
                {
                    this.SuggestedMovies.Add(movieMatch);
                }

                this.NoMovieFound = this.SuggestedMovies.Count == EmptyCollectionCount;
            }
            catch
            {
                this.SuggestedMovies.Clear();
            }
        }

        [RelayCommand(CanExecute = nameof(CanStartScrape))]
        private async Task StartScrapeAsync()
        {
            if (this.SelectedMovie is null)
            {
                return;
            }

            this.IsScraping = true;
            this.StatusText = StatusScraping;
            this.StartScrapeCommand.NotifyCanExecuteChanged();

            try
            {
                await this.ingestionService.RunScrapeJobAsync(
                    this.SelectedMovie,
                    this.MaxResults,
                    onLogEntry: async logEntry =>
                    {
                        // Dispatch to UI thread
#if !IS_TEST_PROJECT
                        Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread()?.TryEnqueue(() =>
                        {
                            this.LogEntries.Insert(TopLogEntryIndex, logEntry);
                        });
#else
                        this.LogEntries.Insert(TopLogEntryIndex, logEntry);
#endif
                        await Task.CompletedTask;
                    });
            }
            catch
            {
                // Errors are already logged inside RunScrapeJobAsync
            }
            finally
            {
                this.IsScraping = false;
                this.StatusText = StatusIdle;
                this.StartScrapeCommand.NotifyCanExecuteChanged();
                await this.RefreshAsync();
            }
        }

        private bool CanStartScrape() => !this.IsScraping && this.SelectedMovie is not null;

        [RelayCommand]
        private async Task RefreshAsync()
        {
            try
            {
                MovieApp.DataLayer.Models.DashboardStatsModel stats = await this.repository.GetDashboardStatsAsync();
                this.TotalMovies = stats.TotalMovies;
                this.TotalReels = stats.TotalReels;
                this.TotalJobs = stats.TotalJobs;
                this.RunningJobs = stats.RunningJobs;
                this.CompletedJobs = stats.CompletedJobs;
                this.FailedJobs = stats.FailedJobs;

                IList<ScrapeJobLog> logs = await this.repository.GetAllLogsAsync();
                this.LogEntries.Clear();
                foreach (ScrapeJobLog log in logs)
                {
                    this.LogEntries.Add(log);
                }

                // Table viewers
                IList<Movie> movies = await this.repository.GetAllMoviesAsync();
                this.MovieTableItems.Clear();

                foreach (Movie movie in movies)
                {
                    this.MovieTableItems.Add(movie);
                }

                IList<Reel> reels = await this.repository.GetAllReelsAsync();
                this.ReelTableItems.Clear();

                foreach (Reel reel in reels)
                {
                    this.ReelTableItems.Add(reel);
                }
            }
            catch
            {
                // Database may not be available during development
            }
        }
    }
}

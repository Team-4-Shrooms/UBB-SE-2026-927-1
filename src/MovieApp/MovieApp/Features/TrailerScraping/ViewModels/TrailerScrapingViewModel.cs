using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.TrailerScraping;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Features.TrailerScraping.ViewModels
{
    /// <summary>
    /// ViewModel for the Trailer Scraping admin dashboard.
    /// Handles movie autocomplete, scrape execution, and log display.
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
        private readonly IMovieService movieService;

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
        /// <param name="movieService">The movie service used for search and listing.</param>
        public TrailerScrapingViewModel(
            IVideoIngestionService ingestionService,
            IMovieService movieService)
        {
            this.ingestionService = ingestionService;
            this.movieService = movieService;
        }

        /// <summary>
        /// Gets the collection of movies suggested by the search query.
        /// </summary>
        public ObservableCollection<Movie> SuggestedMovies { get; } = new();

        /// <summary>
        /// Gets the available options for the maximum number of search results.
        /// </summary>
        public List<int> MaxResultsOptions { get; } = new() { 5, 10, 15, 25, 50 };

        /// <summary>
        /// Gets the collection of scrape job logs.
        /// </summary>
        public ObservableCollection<ScrapeJobLog> LogEntries { get; } = new();

        /// <summary>
        /// Gets the collection of all movies for the data table.
        /// </summary>
        public ObservableCollection<Movie> MovieTableItems { get; } = new();

        /// <summary>
        /// Gets the collection of all reels for the data table.
        /// </summary>
        public ObservableCollection<Reel> ReelTableItems { get; } = new();

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
        public async Task InitializeAsync()
        {
            await this.RefreshAsync();
        }

        /// <summary>
        /// Queries movies via IMovieService for case-insensitive matches.
        /// </summary>
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
                var matches = await this.movieService.SearchMoviesAsync(query);
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
                // Compute job stats from the ingestion service
                IList<ScrapeJob> jobs = await this.ingestionService.GetAllJobsAsync();
                this.TotalJobs = jobs.Count;
                this.RunningJobs = 0;
                this.CompletedJobs = 0;
                this.FailedJobs = 0;

                foreach (ScrapeJob job in jobs)
                {
                    if (job.Status == "running")
                    {
                        this.RunningJobs++;
                    }
                    else if (job.Status == "completed")
                    {
                        this.CompletedJobs++;
                    }
                    else if (job.Status == "failed")
                    {
                        this.FailedJobs++;
                    }
                }

                // Movie list from movie service
                var movies = await this.movieService.GetAllMoviesAsync();
                this.TotalMovies = movies.Count;
                this.MovieTableItems.Clear();
                foreach (Movie movie in movies)
                {
                    this.MovieTableItems.Add(movie);
                }

                // Reels table not available via proxy; stubbed empty
                this.TotalReels = 0;
                this.ReelTableItems.Clear();
            }
            catch
            {
                // API may not be available
            }
        }
    }
}

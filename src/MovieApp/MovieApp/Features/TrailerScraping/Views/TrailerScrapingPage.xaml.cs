namespace MovieApp.Features.TrailerScraping.Views
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using MovieApp.DataLayer.Models;
    using MovieApp.Features.TrailerScraping.ViewModels;
    using CommunityToolkit.Mvvm.DependencyInjection;

    /// <summary>
    /// The page responsible for the trailer scraping admin dashboard.
    /// </summary>
    public sealed partial class TrailerScrapingPage : Page
    {
        private const int SingleMatchCount = 1;
        private const int FirstItemIndex = 0;
        private const string SelectedMovieFormat = "Selected: {0} ({1}) — {2}";

        /// <summary>
        /// Initializes a new instance of the <see cref="TrailerScrapingPage"/> class.
        /// </summary>
        public TrailerScrapingPage()
        {
            this.ViewModel = Ioc.Default.GetRequiredService<TrailerScrapingViewModel>();
            this.InitializeComponent();

            this.MaxResultsCombo.ItemsSource = this.ViewModel.MaxResultsOptions;
            this.MaxResultsCombo.SelectedItem = this.ViewModel.MaxResults;

            this.MaxResultsCombo.SelectionChanged += (comboSender, selectionEventArgs) =>
            {
                if (this.MaxResultsCombo.SelectedItem is int selectedValue)
                {
                    this.ViewModel.MaxResults = selectedValue;
                }
            };
        }

        /// <summary>
        /// Gets the ViewModel for the trailer scraping page.
        /// </summary>
        public TrailerScrapingViewModel ViewModel { get; }

        private async void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await this.ViewModel.InitializeAsync();
        }

        /// <summary>
        /// Fires as the user types — queries the Movie table for matches.
        /// </summary>
        /// <param name="sender">The auto suggest box.</param>
        /// <param name="textChangedEventArgs">The text changed event arguments.</param>
        private async void MovieSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs textChangedEventArgs)
        {
            if (textChangedEventArgs.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                await this.ViewModel.SearchMoviesCommand.ExecuteAsync(sender.Text);

                sender.ItemsSource = this.ViewModel.SuggestedMovies;

                this.NoMatchWarning.Visibility = this.ViewModel.NoMovieFound
                    ? Visibility.Visible
                    : Visibility.Collapsed;

                this.SelectedMovieCard.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Fires when the user picks a suggestion from the dropdown.
        /// </summary>
        /// <param name="sender">The auto suggest box.</param>
        /// <param name="suggestionChosenEventArgs">The suggestion chosen event arguments.</param>
        private void MovieSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs suggestionChosenEventArgs)
        {
            if (suggestionChosenEventArgs.SelectedItem is Movie movie)
            {
                sender.Text = movie.Title;
                this.ViewModel.SelectMovie(movie);

                this.SelectedMovieText.Text = string.Format(SelectedMovieFormat, movie.Title, movie.ReleaseYear, movie.Genre);
                this.SelectedMovieCard.Visibility = Visibility.Visible;
                this.NoMatchWarning.Visibility = Visibility.Collapsed;
            }
        }

        private void MovieSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs querySubmittedEventArgs)
        {
            if (querySubmittedEventArgs.ChosenSuggestion is Movie movie)
            {
                this.ViewModel.SelectMovie(movie);

                this.SelectedMovieText.Text = string.Format(SelectedMovieFormat, movie.Title, movie.ReleaseYear, movie.Genre);
                this.SelectedMovieCard.Visibility = Visibility.Visible;
                this.NoMatchWarning.Visibility = Visibility.Collapsed;
            }
            else if (this.ViewModel.SuggestedMovies.Count == SingleMatchCount)
            {
                Movie onlyMovie = this.ViewModel.SuggestedMovies[FirstItemIndex];
                sender.Text = onlyMovie.Title;
                this.ViewModel.SelectMovie(onlyMovie);

                this.SelectedMovieText.Text = string.Format(SelectedMovieFormat, onlyMovie.Title, onlyMovie.ReleaseYear, onlyMovie.Genre);
                this.SelectedMovieCard.Visibility = Visibility.Visible;
                this.NoMatchWarning.Visibility = Visibility.Collapsed;
            }
        }
    }
}

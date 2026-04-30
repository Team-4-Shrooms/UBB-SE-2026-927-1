using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Features.Shared.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;

namespace MovieApp.Features.MovieDetail.Views
{
    public sealed partial class MovieReviewsPage : Page
    {
        private Movie? _movie;
        private readonly IReviewRepository _reviewRepository = App.Services.GetRequiredService<IReviewRepository>();
        private readonly IUserRepository _userRepository = App.Services.GetRequiredService<IUserRepository>();
        private readonly IReviewService _reviewService = App.Services.GetRequiredService<IReviewService>();
        private readonly IMovieService _movieService = App.Services.GetRequiredService<IMovieService>();

        public MovieReviewsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is MovieReviewsNavArgs args)
            {
                _movie = args.Movie;
            }

            TitleBlock.Text = _movie == null ? "Reviews" : $"Reviews - {_movie.Title}";
            AddReviewButton.IsEnabled = SessionManager.IsLoggedIn;
            
            await LoadReviewsAsync();
        }

        private void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async Task LoadReviewsAsync()
        {
            if (_movie == null)
            {
                return;
            }

            List<MovieReview> reviews = await _reviewRepository.GetReviewsForMovieAsync(_movie.Id);
            ReviewsList.ItemsSource = reviews.OrderByDescending(r => r.CreatedAt).ToList();
        }

        private async void AddReviewButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (!SessionManager.IsLoggedIn || _movie == null)
            {
                return;
            }

            while (true)
            {
                var ratingBox = new TextBox { PlaceholderText = "1 - 10", Width = 240 };
                var commentBox = new TextBox { PlaceholderText = "Comment (optional)", AcceptsReturn = true, TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap, Height = 90 };

                var content = new StackPanel { Spacing = 10 };
                content.Children.Add(new TextBlock { Text = "Rating", Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White) });
                content.Children.Add(ratingBox);
                content.Children.Add(new TextBlock { Text = "Comment", Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White), Margin = new Microsoft.UI.Xaml.Thickness(0, 8, 0, 0) });
                content.Children.Add(commentBox);

                var dialog = new ContentDialog
                {
                    Title = "Add review",
                    Content = content,
                    PrimaryButtonText = "Submit",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = XamlRoot
                };

                var result = await dialog.ShowAsync();
                if (result != ContentDialogResult.Primary) return;

                if (!TryParseRating(ratingBox.Text, out var rating, out var error))
                {
                    var err = new ContentDialog
                    {
                        Title = "Invalid rating",
                        Content = error,
                        PrimaryButtonText = "OK",
                        XamlRoot = XamlRoot
                    };
                    await err.ShowAsync();
                    continue;
                }

                await _reviewService.PostReviewAsync(_movie.Id, SessionManager.CurrentUserID, rating, commentBox.Text);
                
                await LoadReviewsAsync();
                return;
            }
        }

        private static bool TryParseRating(string? text, out int rating, out string error)
        {
            rating = 0;
            error = "";
            string trimmedText = (text ?? "").Trim();
            if (!int.TryParse(trimmedText, out rating) || rating < 1 || rating > 10)
            {
                error = "Please enter a rating between 1 and 10.";
                return false;
            }

            return true;
        }
    }

    public sealed class MovieReviewsNavArgs
    {
        public Movie? Movie { get; init; }
    }
}

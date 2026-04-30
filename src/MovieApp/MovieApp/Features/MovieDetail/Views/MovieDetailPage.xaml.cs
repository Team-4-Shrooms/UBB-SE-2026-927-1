using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Features.Shared.Models;
using MovieApp.Features.Events.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MovieApp.Features.MovieDetail.Views
{
    public sealed partial class MovieDetailPage : Page
    {
        private Movie? _movie;
        private decimal _appliedDiscount = 0;
        private readonly IMovieRepository _movieRepository = App.Services.GetRequiredService<IMovieRepository>();
        private readonly IActiveSalesRepository _activeSalesRepository = App.Services.GetRequiredService<IActiveSalesRepository>();
        private readonly IReviewRepository _reviewRepository = App.Services.GetRequiredService<IReviewRepository>();
        private readonly IUserRepository _userRepository = App.Services.GetRequiredService<IUserRepository>();
        private readonly IMovieService _movieService = App.Services.GetRequiredService<IMovieService>();
        private readonly IActiveSalesService _activeSalesService = App.Services.GetRequiredService<IActiveSalesService>();
        private readonly IReviewService _reviewService = App.Services.GetRequiredService<IReviewService>();
        private readonly AppDbContext _context = App.Services.GetRequiredService<AppDbContext>();

        public MovieDetailPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is MovieDetailNavArgs args)
            {
                _movie = args.Movie;
            }

            if (_movie == null)
            {
                return;
            }

            Dictionary<int, decimal> discountMap = await _activeSalesService.GetBestDiscountPercentByMovieIdAsync();
            if (discountMap.TryGetValue(_movie.Id, out decimal discount))
            {
                _appliedDiscount = discount;
            }

            PopulateUI();
            await RefreshBuyButtonStateAsync();

            string tooltip = await BuildStarDistributionTooltipAsync(_movie.Id);
            ToolTipService.SetToolTip(ReviewsButton, tooltip);
        }

        private void PopulateUI()
        {
            if (_movie == null)
            {
                return;
            }

            TitleBlock.Text = _movie.Title;
            DescriptionBlock.Text = string.IsNullOrEmpty(_movie.Description) ? "—" : _movie.Description;
            RatingBlock.Text = $"Rating: {_movie.Rating:0.0} / 10";

            UpdatePriceDisplay();
            TrySetPoster(_movie.PosterUrl);
        }

        private void UpdatePriceDisplay()
        {
            if (_movie == null)
            {
                return;
            }

            decimal effectivePrice = GetEffectivePrice();
            PriceBlock.Text = $"${effectivePrice:F2}";

            if (_appliedDiscount > 0)
            {
                OriginalPriceBlock.Visibility = Visibility.Visible;
                OriginalPriceBlock.Text = $"${_movie.Price:F2}";
            }
            else
            {
                OriginalPriceBlock.Visibility = Visibility.Collapsed;
            }
        }

        private decimal GetEffectivePrice()
        {
            if (_movie == null)
            {
                return 0;
            }

            return _movie.Price * (1 - _appliedDiscount / 100);
        }

        private void TrySetPoster(string? url)
        {
            PosterImage.Source = null;
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            try
            {
                PosterImage.Source = new BitmapImage(new Uri(url, UriKind.Absolute));
            }
            catch
            {
            }
        }

        private async Task RefreshBuyButtonStateAsync()
        {
            if (_movie == null)
            {
                return;
            }

            int userId = SessionManager.CurrentUserID;
            bool loggedIn = SessionManager.IsLoggedIn;
            bool owned = await _movieRepository.UserOwnsMovieAsync(userId, _movie.Id);
            decimal balance = SessionManager.CurrentUserBalance;

            decimal effectivePrice = GetEffectivePrice();
            bool insufficient = loggedIn && !owned && balance < effectivePrice;

            if (owned)
            {
                BuyMovieButton.Content = "Owned";
                BuyMovieButton.IsEnabled = false;
                ToolTipService.SetToolTip(BuyMovieButton, null);
                BuyMovieButton.Opacity = 1;
                return;
            }

            BuyMovieButton.Content = "Buy movie";

            if (!loggedIn)
            {
                BuyMovieButton.IsEnabled = false;
                ToolTipService.SetToolTip(BuyMovieButton, "You must be logged in to make a purchase.");
                BuyMovieButton.Opacity = 0.55;
                return;
            }

            if (insufficient)
            {
                BuyMovieButton.IsEnabled = false;
                ToolTipService.SetToolTip(BuyMovieButton, $"Insufficient funds. Balance: {balance:C} — Price: {effectivePrice:C}");
                BuyMovieButton.Opacity = 0.55;
                return;
            }

            BuyMovieButton.IsEnabled = true;
            ToolTipService.SetToolTip(BuyMovieButton, null);
            BuyMovieButton.Opacity = 1;
        }

        private async void BuyMovieButton_Click(object sender, RoutedEventArgs e)
        {
            if (_movie == null || !SessionManager.IsLoggedIn) return;

            var effectivePrice = GetEffectivePrice();

            var confirm = new ContentDialog
            {
                Title = "Confirm purchase",
                Content = $"Buy \"{_movie.Title}\" for {effectivePrice:C}? This will be charged to your balance.",
                PrimaryButtonText = "Buy",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot
            };

            if (await confirm.ShowAsync() != ContentDialogResult.Primary) return;

            try
            {
                var userId = SessionManager.CurrentUserID;
                var user = await _context.Users.FindAsync(userId) 
                    ?? throw new Exception("User not found.");
                
                var movie = await _context.Movies.FindAsync(_movie.Id)
                    ?? throw new Exception("Movie not found.");

                if (user.Balance < effectivePrice)
                    throw new InvalidOperationException("Insufficient balance.");

                user.Balance -= effectivePrice;

                var ownership = new OwnedMovie
                {
                    User = user,
                    Movie = movie,
                    PurchaseDate = DateTime.UtcNow
                };

                var transaction = new Transaction
                {
                    Buyer = user,
                    Movie = movie,
                    Amount = -effectivePrice,
                    Type = "MoviePurchase",
                    Status = "Completed",
                    Timestamp = DateTime.UtcNow
                };

                _context.OwnedMovies.Add(ownership);
                _context.Transactions.Add(transaction);
                
                await _context.SaveChangesAsync();
                
                SessionManager.CurrentUserBalance = user.Balance;

                await RefreshBuyButtonStateAsync();

                var dialog = new ContentDialog
                {
                    Title = "Purchase successful",
                    Content = $"You now own \"{_movie.Title}\". It has been added to your inventory.",
                    PrimaryButtonText = "OK",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = XamlRoot
                };
                await dialog.ShowAsync();
                
                Frame.Navigate(typeof(Features.Inventory.Views.InventoryPage));
            }
            catch (Exception exception)
            {
                var error = new ContentDialog
                {
                    Title = "Error",
                    Content = exception.Message,
                    PrimaryButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await error.ShowAsync();
            }
        }

        private void ReviewsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_movie == null) return;
            Frame.Navigate(typeof(MovieReviewsPage), new MovieReviewsNavArgs { Movie = _movie });
        }

        private void EventsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_movie == null) return;
            Frame.Navigate(typeof(MovieEventsPage), new MovieEventsNavArgs { Movie = _movie });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
            else Frame.Navigate(typeof(Features.MovieCatalog.Views.MovieCatalogPage));
        }

        private async Task<string> BuildStarDistributionTooltipAsync(int movieId)
        {
            var counts = await _reviewService.GetStarRatingBucketsAsync(movieId);
            if (counts.Sum() == 0) return "No reviews yet.";
 
            var lines = new List<string> { "Rating distribution:" };
            for (var i = 10; i >= 1; i--)
                lines.Add($"{i}: {counts[i]}");
 
            return string.Join("\n", lines);
        }
    }
}

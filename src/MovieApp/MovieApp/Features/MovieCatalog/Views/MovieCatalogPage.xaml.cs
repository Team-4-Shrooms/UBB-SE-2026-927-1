using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Features.Marketplace.ViewModels;
using MovieApp.Features.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using MovieApp.Features.Marketplace.ViewModels;
using System.Threading.Tasks;
using System;
using MovieApp.WebApi.Data;

namespace MovieApp.Features.MovieCatalog.Views
{
    public sealed class MovieCatalogItem
    {
        public Movie Movie { get; }
        public int Id => Movie.Id;
        public string Title => Movie.Title;
        public string? ImageUrl => Movie.PosterUrl;
        public decimal Rating => Movie.Rating;
        public int ReviewCount { get; }

        public string RatingAndReviewCountText => $"Ratings ({ReviewCount}): {Rating:0.0} / 10";

        public bool IsOnSale => Movie.HasActiveSale;

        public string OriginalPriceText => $"$ {Movie.Price:0.00}";

        public string CurrentPriceText => $"$ {Movie.GetEffectivePrice():0.00}";

        public Microsoft.UI.Xaml.Visibility SaleVisibility => IsOnSale
            ? Microsoft.UI.Xaml.Visibility.Visible
            : Microsoft.UI.Xaml.Visibility.Collapsed;

        public Microsoft.UI.Xaml.Media.SolidColorBrush PriceColor => IsOnSale
            ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.IndianRed)
            : new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 29, 185, 84));

        public MovieCatalogItem(Movie movie, int reviewCount)
        {
            Movie = movie;
            ReviewCount = reviewCount;
        }
    }

    public sealed partial class MovieCatalogPage : Page
    {
        private readonly IMovieRepository _movieRepo = App.Services.GetRequiredService<IMovieRepository>();
        private readonly IActiveSalesRepository _salesRepo = App.Services.GetRequiredService<IActiveSalesRepository>();
        private readonly IReviewRepository _reviewRepo = App.Services.GetRequiredService<IReviewRepository>();
        private readonly AppDbContext _context = App.Services.GetRequiredService<AppDbContext>();
        
        private List<Movie> _sourceMovies = new();
        private Dictionary<int, int> _reviewCountByMovieId = new();
        
        public FlashSaleViewModel FlashSaleVM { get; } = App.Services.GetRequiredService<FlashSaleViewModel>();
        private bool _showOnlySales;
        private FlashSaleViewModel? _flashSaleVm;

        public MovieCatalogPage()
        {
            InitializeComponent();
            SearchBox.TextChanged += (_, _) => ApplyFilterAndSort();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is MovieCatalogNavArgs args)
            {
                _showOnlySales = args.ShowOnlySales;
            }

            await LoadDiscountedMoviesAsync();
 
            SortAscPrice.IsChecked = true;
            ApplyFilterAndSort();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs @event)
        {
            base.OnNavigatedFrom(@event);
            if (_flashSaleVm != null)
                _flashSaleVm.PropertyChanged -= FlashSaleVm_PropertyChanged!;
        }

        private void SortOption_Changed(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => ApplyFilterAndSort();

        private void ApplyFilterAndSort()
        {
            string query = (SearchBox.Text ?? "").Trim().ToLower();
            IEnumerable<Movie> list = string.IsNullOrEmpty(query) ? _sourceMovies
                                                : _sourceMovies.Where(movie => movie.Title.ToLower().Contains(query));

            if (SortAscPrice.IsChecked == true)
            {
                list = list.OrderBy(movie => movie.GetEffectivePrice());
            }
            else if (SortDescPrice.IsChecked == true)
            {
                list = list.OrderByDescending(movie => movie.GetEffectivePrice());
            }
            else if (SortHighRating.IsChecked == true)
            {
                list = list.OrderByDescending(movie => movie.Rating);
            }
            else if (SortLowRating.IsChecked == true)
            {
                list = list.OrderBy(movie => movie.Rating);
            }
            else
            {
                list = list.OrderBy(movie => movie.Title);
            }

            var orderedMovies = list.ToList();
            MoviesGrid.ItemsSource = orderedMovies
                .Select(movie => new MovieCatalogItem(movie, _reviewCountByMovieId.TryGetValue(movie.Id, out int reviewCount) ? reviewCount : 0))
                .ToList();
        }

        private void MovieCard_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is not Border border)
            {
                return;
            }

            border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 29, 185, 84));
            border.Background = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48));
            border.RenderTransform = new ScaleTransform { ScaleX = 1.03, ScaleY = 1.03 };
        }

        private void MovieCard_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is not Border border)
            {
                return;
            }

            border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
            border.Background = new SolidColorBrush(Color.FromArgb(255, 42, 42, 42));
            border.RenderTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
        }

        private void MoviesGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is MovieCatalogItem item)
            {
                Frame.Navigate(typeof(Features.MovieDetail.Views.MovieDetailPage), new MovieDetailNavArgs { Movie = item.Movie });
            }
        }

        private async Task LoadDiscountedMoviesAsync()
        {
            var allMovies = await _movieRepo.SearchMoviesAsync("", 100);
            var currentSales = await _salesRepo.GetCurrentSalesAsync();
            
            foreach (var movie in allMovies)
            {
                var sale = currentSales.FirstOrDefault(sale => sale.Movie.Id == movie.Id);
                movie.ActiveSaleDiscountPercent = sale?.DiscountPercentage;
            }
 
            _context.ChangeTracker.Clear();
 
            var movieIds = allMovies.Select(movie => movie.Id).ToList();
            _reviewCountByMovieId = await _reviewRepo.GetReviewCountsAsync(movieIds);
 
            var onSaleIds = currentSales.Select(sale => sale.Movie.Id).ToHashSet();
            _sourceMovies = allMovies.Where(movie => onSaleIds.Contains(movie.Id)).ToList();
 
            if (currentSales.Any())
            {
                var latestSale = currentSales.OrderByDescending(sale => sale.EndTime).First();
                if (_flashSaleVm == null)
                {
                    _flashSaleVm = new FlashSaleViewModel(latestSale.EndTime, () => {
                         DispatcherQueue.TryEnqueue(() => ApplyCatalogDeactivation(false));
                    });
                    _flashSaleVm.PropertyChanged += FlashSaleVm_PropertyChanged!;
                }
                ApplyCatalogDeactivation(true);
            }
            else
            {
                ApplyCatalogDeactivation(false);
            }
        }

        private void FlashSaleVm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(FlashSaleViewModel.IsActive))
                return;

            DispatcherQueue.TryEnqueue(async () =>
            {
                ApplyCatalogDeactivation(_flashSaleVm?.IsActive ?? false);
                if (_flashSaleVm?.IsActive ?? false)
                {
                    await LoadDiscountedMoviesAsync();
                    ApplyFilterAndSort();
                }
            });
        }

        private void ApplyCatalogDeactivation(bool isSaleActive)
        {
            if (isSaleActive)
            {
                FlashSaleEndedText.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                MoviesGrid.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                MoviesGrid.IsEnabled = true;
                MoviesGrid.Opacity = 1.0;
                return;
            }

            MoviesGrid.ItemsSource = null;
            _sourceMovies = new List<Movie>();
            FlashSaleEndedText.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            MoviesGrid.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }
}

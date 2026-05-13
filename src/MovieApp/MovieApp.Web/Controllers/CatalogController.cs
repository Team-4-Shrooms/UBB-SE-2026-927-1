using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.Models;

namespace MovieApp.Web.Controllers;

public sealed class CatalogController : Controller
{
    private readonly IMovieService _movieService;
    private readonly IReviewService _reviewService;
    private readonly IActiveSalesService _activeSalesService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemoryCache _cache;

    public CatalogController(
        IMovieService movieService,
        IReviewService reviewService,
        IActiveSalesService activeSalesService,
        ICurrentUserService currentUserService,
        IMemoryCache cache)
    {
        _movieService = movieService;
        _reviewService = reviewService;
        _activeSalesService = activeSalesService;
        _currentUserService = currentUserService;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? genre, decimal? minRating, string? sort)
    {
        var movies = string.IsNullOrWhiteSpace(search)
            ? await _movieService.GetAllMoviesAsync()
            : await _movieService.SearchMoviesAsync(search);

        var genres = await _cache.GetOrCreateAsync("catalog:genres", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            var allMovies = await _movieService.GetAllMoviesAsync();
            return allMovies.Select(m => m.Genre).Where(g => !string.IsNullOrEmpty(g)).Distinct().OrderBy(g => g).ToList();
        });

        var activeSales = await _cache.GetOrCreateAsync("sales:active", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _activeSalesService.GetBestDiscountPercentByMovieIdAsync();
        });

        if (activeSales != null)
        {
            foreach (var movie in movies)
            {
                if (activeSales.TryGetValue(movie.Id, out var discount))
                {
                    movie.ActiveSaleDiscountPercent = discount;
                }
            }
        }

        var filteredMovies = movies.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(genre))
        {
            filteredMovies = filteredMovies.Where(m => m.Genre == genre);
        }
        if (minRating.HasValue)
        {
            filteredMovies = filteredMovies.Where(m => m.Rating >= minRating.Value);
        }

        filteredMovies = sort switch
        {
            "price_asc"    => filteredMovies.OrderBy(m => m.GetEffectivePrice()),
            "price_desc"   => filteredMovies.OrderByDescending(m => m.GetEffectivePrice()),
            "rating_desc"  => filteredMovies.OrderByDescending(m => m.Rating),
            "rating_asc"   => filteredMovies.OrderBy(m => m.Rating),
            _              => filteredMovies.OrderBy(m => m.Title),
        };

        var viewModel = new CatalogIndexViewModel
        {
            Movies = filteredMovies.ToList(),
            Genres = genres ?? new List<string>(),
            Filter = new CatalogFilter
            {
                Search = search,
                Genre = genre,
                MinRating = minRating,
                Sort = sort,
            },
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        var activeSales = await _cache.GetOrCreateAsync("sales:active", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _activeSalesService.GetBestDiscountPercentByMovieIdAsync();
        });

        if (activeSales != null && activeSales.TryGetValue(movie.Id, out var discount))
        {
            movie.ActiveSaleDiscountPercent = discount;
        }

        var reviews = await _reviewService.GetReviewsForMovieAsync(id);
        var buckets = await _reviewService.GetStarRatingBucketsAsync(id);

        var userId = _currentUserService.UserId;
        var userOwnsMovie = userId > 0 && await _movieService.UserOwnsMovieAsync(userId, id);

        var viewModel = new CatalogDetailViewModel
        {
            Movie = movie,
            Reviews = reviews.OrderByDescending(r => r.CreatedAt).ToList(),
            StarRatingBuckets = buckets,
            Form = new AddReviewForm { MovieId = id },
            UserOwnsMovie = userOwnsMovie,
            IsLoggedIn = userId > 0,
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PurchaseMovie(int movieId, decimal price)
    {
        var userId = _currentUserService.UserId;
        if (userId <= 0)
        {
            TempData["PurchaseError"] = "You must be logged in to purchase a movie.";
            return RedirectToAction(nameof(Detail), new { id = movieId });
        }

        try
        {
            await _movieService.PurchaseMovieAsync(userId, movieId, price);
            TempData["PurchaseSuccess"] = "Purchase successful! The movie has been added to your inventory.";
        }
        catch (Exception ex)
        {
            TempData["PurchaseError"] = ex.Message;
        }

        return RedirectToAction(nameof(Detail), new { id = movieId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(AddReviewForm form)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please provide a valid rating and comment.";
            return RedirectToAction(nameof(Detail), new { id = form.MovieId });
        }

        _cache.Remove("catalog:genres");

        await _reviewService.PostReviewAsync(form.MovieId, _currentUserService.UserId, form.StarRating, form.Comment);

        return RedirectToAction(nameof(Detail), new { id = form.MovieId });
    }
}

using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class ActiveSalesRepositoryTests
    {
        [Fact]
        public void GetCurrentSales_NoSales_ReturnsEmptyList()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            ActiveSalesRepository repository = new ActiveSalesRepository(context);

            Assert.Empty(repository.GetCurrentSales());
        }

        [Fact]
        public void GetCurrentSales_OnlyReturnsSalesWithinWindow_ReturnsSingleSale()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            DateTime now = DateTime.UtcNow;
            Movie currentMovie = BuildMovie();
            Movie expiredMovie = BuildMovie();
            context.Movies.AddRange(currentMovie, expiredMovie);
            context.ActiveSales.AddRange(
                BuildSale(currentMovie, 20m, now.AddDays(-1), now.AddDays(1)),
                BuildSale(expiredMovie, 50m, now.AddDays(-10), now.AddDays(-2)));
            context.SaveChanges();

            ActiveSalesRepository repository = new ActiveSalesRepository(context);
            List<ActiveSale> sales = repository.GetCurrentSales();

            Assert.Single(sales);
        }

        [Fact]
        public void GetCurrentSales_OnlyReturnsSalesWithinWindow_ReturnsSaleWithCorrectDiscount()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            DateTime now = DateTime.UtcNow;
            Movie currentMovie = BuildMovie();
            Movie expiredMovie = BuildMovie();
            context.Movies.AddRange(currentMovie, expiredMovie);
            context.ActiveSales.AddRange(
                BuildSale(currentMovie, 20m, now.AddDays(-1), now.AddDays(1)),
                BuildSale(expiredMovie, 50m, now.AddDays(-10), now.AddDays(-2)));
            context.SaveChanges();

            ActiveSalesRepository repository = new ActiveSalesRepository(context);
            List<ActiveSale> sales = repository.GetCurrentSales();

            Assert.Equal(20m, sales[0].DiscountPercentage);
        }

        [Fact]
        public void GetBestDiscountPercentByMovieId_ReturnsEntryPerMovie()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            DateTime now = DateTime.UtcNow;
            Movie movieA = BuildMovie();
            Movie movieB = BuildMovie();
            context.Movies.AddRange(movieA, movieB);
            context.ActiveSales.AddRange(
                BuildSale(movieA, 20m, now.AddDays(-1), now.AddDays(1)),
                BuildSale(movieA, 35m, now.AddDays(-1), now.AddDays(2)),
                BuildSale(movieB, 10m, now.AddDays(-1), now.AddDays(1)));
            context.SaveChanges();

            ActiveSalesRepository repository = new ActiveSalesRepository(context);
            Dictionary<int, decimal> result = repository.GetBestDiscountPercentByMovieId();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetBestDiscountPercentByMovieId_PicksHighestDiscountForMovieWithMultipleSales()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            DateTime now = DateTime.UtcNow;
            Movie movieA = BuildMovie();
            Movie movieB = BuildMovie();
            context.Movies.AddRange(movieA, movieB);
            context.ActiveSales.AddRange(
                BuildSale(movieA, 20m, now.AddDays(-1), now.AddDays(1)),
                BuildSale(movieA, 35m, now.AddDays(-1), now.AddDays(2)),
                BuildSale(movieB, 10m, now.AddDays(-1), now.AddDays(1)));
            context.SaveChanges();

            ActiveSalesRepository repository = new ActiveSalesRepository(context);
            Dictionary<int, decimal> result = repository.GetBestDiscountPercentByMovieId();

            Assert.Equal(35m, result[movieA.Id]);
        }

        [Fact]
        public void GetBestDiscountPercentByMovieId_ReturnsCorrectDiscountForSingleSaleMovie()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            DateTime now = DateTime.UtcNow;
            Movie movieA = BuildMovie();
            Movie movieB = BuildMovie();
            context.Movies.AddRange(movieA, movieB);
            context.ActiveSales.AddRange(
                BuildSale(movieA, 20m, now.AddDays(-1), now.AddDays(1)),
                BuildSale(movieA, 35m, now.AddDays(-1), now.AddDays(2)),
                BuildSale(movieB, 10m, now.AddDays(-1), now.AddDays(1)));
            context.SaveChanges();

            ActiveSalesRepository repository = new ActiveSalesRepository(context);
            Dictionary<int, decimal> result = repository.GetBestDiscountPercentByMovieId();

            Assert.Equal(10m, result[movieB.Id]);
        }

        private static Movie BuildMovie()
        {
            return new Movie
            {
                Title = "Inception",
                Description = "desc",
                PrimaryGenre = "Drama",
                Synopsis = "synopsis",
                Price = 10m,
                Rating = 0m,
                ReleaseYear = 2010
            };
        }

        private static ActiveSale BuildSale(Movie movie, decimal discount, DateTime start, DateTime end)
        {
            return new ActiveSale
            {
                Movie = movie,
                DiscountPercentage = discount,
                StartTime = start,
                EndTime = end
            };
        }
    }
}

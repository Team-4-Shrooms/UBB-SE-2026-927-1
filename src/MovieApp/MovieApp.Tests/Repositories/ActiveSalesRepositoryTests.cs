using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class ActiveSalesRepositoryTests
    {
        [Fact]
        public void GetCurrentSales_NoSalesExist_ReturnsEmptyList()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            ActiveSalesRepository repository = new ActiveSalesRepository(context);

            List<ActiveSale> currentSales = repository.GetCurrentSales();

            Assert.Empty(currentSales);
        }

        [Fact]
        public void GetCurrentSales_OneCurrentOneExpired_ReturnsSingleSale()
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
            List<ActiveSale> currentSales = repository.GetCurrentSales();

            Assert.Single(currentSales);
        }

        [Fact]
        public void GetCurrentSales_OneCurrentOneExpired_ReturnedSaleHasCorrectDiscount()
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
            List<ActiveSale> currentSales = repository.GetCurrentSales();

            Assert.Equal(20m, currentSales[0].DiscountPercentage);
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

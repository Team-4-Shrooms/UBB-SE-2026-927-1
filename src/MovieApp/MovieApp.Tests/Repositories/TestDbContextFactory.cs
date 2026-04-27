using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MovieApp.Logic.Data;

namespace MovieApp.Tests.Repositories
{
    internal static class TestDbContextFactory
    {
        public static WebAPIDbContext Create()
        {
            DbContextOptions<WebAPIDbContext> options = new DbContextOptionsBuilder<WebAPIDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(builder => builder.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new WebAPIDbContext(options);
        }
    }
}

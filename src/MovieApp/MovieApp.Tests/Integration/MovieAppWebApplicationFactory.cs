using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Interfaces;

namespace MovieApp.Tests.Integration
{
    /// <summary>
    /// Custom factory that replaces the SQL Server database with an
    /// EF Core InMemory database so integration tests run without
    /// any external infrastructure. The DataSeeder from Program.cs
    /// will run automatically against this InMemory database.
    /// </summary>
    public class MovieAppWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = "IntegrationTestDb_" + Guid.NewGuid().ToString("N");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove all AppDbContext-related registrations.
                ServiceDescriptor? existingDbContextDescriptor = services
                    .SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (existingDbContextDescriptor is not null)
                {
                    services.Remove(existingDbContextDescriptor);
                }

                ServiceDescriptor? existingInterfaceDescriptor = services
                    .SingleOrDefault(descriptor => descriptor.ServiceType == typeof(IMovieAppDbContext));

                if (existingInterfaceDescriptor is not null)
                {
                    services.Remove(existingInterfaceDescriptor);
                }

                // Register a shared InMemory database for this factory instance.
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });

                services.AddScoped<IMovieAppDbContext>(serviceProvider =>
                    serviceProvider.GetRequiredService<AppDbContext>());
            });
        }

        protected override void ConfigureClient(HttpClient client)
        {
            // Use HTTP to avoid HTTPS redirect issues in the test environment.
            client.BaseAddress = new Uri("http://localhost");
        }
    }
}

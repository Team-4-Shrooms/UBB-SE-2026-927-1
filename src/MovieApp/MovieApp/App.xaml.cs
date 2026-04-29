using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Repositories;
using MovieApp.WebApi.Data;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Services;
using MovieApp.Features.Marketplace.ViewModels;
using MovieApp.Features.Wallet.ViewModels;
using System;
using System.Threading.Tasks;

namespace MovieApp
{
    public partial class App : Application
    {
        private Window? _window;

        public static IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            InitializeComponent();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
            
            Task.Run(async () => {
                using var scope = Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<WebApi.Data.AppDbContext>();
                var seeder = new DataLayer.DataSeeder(context);
                await seeder.SeedAsync();
            });
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var connectionString = "Server=localhost\\SQLEXPRESS;Database=MovieApp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IMovieAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<IActiveSalesRepository, ActiveSalesRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            services.AddScoped<IEventRepository, EventRepository>();

            services.AddScoped<IEquipmentService, EquipmentService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IActiveSalesService, ActiveSalesService>();
            services.AddScoped<IReviewService, ReviewService>();

            services.AddTransient<MarketplaceViewModel>();
            services.AddTransient<SellEquipmentViewModel>();
            services.AddTransient<WalletViewModel>();
            
            services.AddTransient<FlashSaleViewModel>(sp => new FlashSaleViewModel(DateTime.Now.AddHours(2)));
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}

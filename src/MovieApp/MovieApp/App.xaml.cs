using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using MovieApp.Logic.Http; // Ensures ApiClient is recognized
using MovieApp.DataLayer.Interfaces.Repositories;


namespace MovieApp
{
    public partial class App : Application
    {
        private Window? _window;

        // 1. Expose the services globally so your ViewModels/Pages can access them
        public static IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            InitializeComponent();
            
            // 2. Initialize the DI container when the app starts
            Services = ConfigureServices();
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 3. Register your ApiClient
            services.AddHttpClient<ApiClient>(client => 
            {
                // Make sure this URL matches your running Web API!
                client.BaseAddress = new Uri("https://localhost:7196/"); 
            });

            // Note: When you get your ViewModels back, you'll register them here!
            // e.g., services.AddTransient<MyViewModel>();

            // 4. Register your Repository
            // services.AddScoped<MovieApp.DataLayer.Interfaces.Repositories.IMovieRepository, MovieApp.Logic.Http.MovieProxyRepository>();
            
            services.AddTransient<IMovieRepository, MovieProxyRepository>();
            services.AddTransient<IActiveSalesRepository, ActiveSalesProxyRepository>();
            services.AddTransient<IAudioLibraryRepository, AudioLibraryProxyRepository>();
            services.AddTransient<IEquipmentRepository, EquipmentProxyRepository>();
            services.AddTransient<IEventRepository, EventProxyRepository>();
            services.AddTransient<IInteractionRepository, InteractionProxyRepository>();
            services.AddTransient<IInventoryRepository, InventoryProxyRepository>();
            services.AddTransient<IMovieTournamentRepository, MovieTournamentProxyRepository>();
            services.AddTransient<IPersonalityMatchRepository, PersonalityMatchProxyRepository>();
            services.AddTransient<IPreferenceRepository, PreferenceProxyRepository>();
            services.AddTransient<IProfileRepository, ProfileProxyRepository>();
            services.AddTransient<IRecommendationRepository, RecommendationProxyRepository>();
            services.AddTransient<IReelRepository, ReelProxyRepository>();
            services.AddTransient<IReviewRepository, ReviewProxyRepository>();
            services.AddTransient<IScrapeJobRepository, ScrapeJobProxyRepository>();
            services.AddTransient<ITransactionRepository, TransactionProxyRepository>();
            services.AddTransient<IUserRepository, UserProxyRepository>();
            services.AddTransient<IVideoStorageRepository, VideoStorageProxyRepository>();
            
            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}

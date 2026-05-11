using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using CommunityToolkit.Mvvm.DependencyInjection;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Proxy;
using MovieApp.Proxy.Services;
using MovieApp.Auth;
using MovieApp.WebApi.Data;
using MovieApp.Features.Marketplace.ViewModels;
using MovieApp.Features.Wallet.ViewModels;

namespace MovieApp
{
    public partial class App : Application
    {
        private Window? _window;

        public static Window MainWindow => ((App)Current)._window!;
        public static IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            InitializeComponent();
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            // Direct DB context — still used by some pages pending full proxy migration
            var connectionString = "Server=localhost\\SQLEXPRESS;Database=MovieApp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            // IMovieAppDbContext alias — needed by repositories that use the interface rather than the concrete type
            services.AddScoped<IMovieAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            // Repositories used directly by pages that have not yet been migrated to proxy services
            services.AddTransient<IReelRepository, ReelRepository>();
            services.AddTransient<IAudioLibraryRepository, AudioLibraryRepository>();
            services.AddTransient<IInventoryRepository, InventoryRepository>();
            services.AddTransient<IEventRepository, EventRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IMovieRepository, MovieRepository>();
            services.AddTransient<IActiveSalesRepository, ActiveSalesRepository>();
            services.AddTransient<IReviewRepository, ReviewRepository>();

            // Auth — login to WebApi and get JWT token.
            // Task.Run avoids deadlocking the WinUI UI thread's sync context.
            var authProvider = new WinUiAuthTokenProvider();
            Task.Run(() => authProvider.InitializeAsync()).GetAwaiter().GetResult();
            services.AddSingleton<IAuthTokenProvider>(authProvider);

            // HTTP client + ApiClient
            var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:4544/") };
            services.AddSingleton(httpClient);
            services.AddSingleton<ApiClient>();

            // Proxy services (replace broken proxy repository registrations)
            services.AddTransient<IMovieService, MovieProxyService>();
            services.AddTransient<IEquipmentService, EquipmentProxyService>();
            services.AddTransient<IActiveSalesService, ActiveSalesProxyService>();
            services.AddTransient<IReviewService, ReviewProxyService>();
            services.AddTransient<IInventoryService, InventoryProxyService>();
            services.AddTransient<IEventService, EventProxyService>();
            services.AddTransient<IProfileService, ProfileProxyService>();
            services.AddTransient<IPersonalityMatchService, PersonalityMatchProxyService>();

            // Reels Upload
            services.AddTransient<MovieApp.Logic.Features.ReelsUpload.IVideoStorageService, VideoStorageProxyService>();
            services.AddTransient<MovieApp.Features.ReelsUpload.ViewModels.ReelsUploadViewModel>();

            // Trailer Scraping
            services.AddTransient<MovieApp.Logic.Features.TrailerScraping.IVideoIngestionService, VideoIngestionProxyService>();
            services.AddTransient<MovieApp.Features.TrailerScraping.ViewModels.TrailerScrapingViewModel>();

            // Reels Editing
            services.AddTransient<MovieApp.Logic.Features.ReelsEditing.IVideoProcessingService, VideoProcessingProxyService>();
            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.ReelsEditingViewModel>();
            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.ReelGalleryViewModel>();
            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.MusicSelectionDialogViewModel>();

            // Movie Swipe
            services.AddTransient<MovieApp.Logic.Features.MovieSwipe.ISwipeService, SwipeProxyService>();
            services.AddTransient<MovieApp.Logic.Features.MovieSwipe.IMovieCardFeedService, MovieCardFeedProxyService>();
            services.AddTransient<MovieApp.Features.MovieSwipe.ViewModels.MovieSwipeViewModel>();

            // Movie Tournament
            services.AddSingleton<MovieApp.Logic.Features.MovieTournament.ITournamentLogicService, TournamentLogicProxyService>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.MovieTournamentViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentMatchViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentSetupViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentWinnerViewModel>();

            // Personality Match
            services.AddTransient<MovieApp.Logic.Features.PersonalityMatch.IPersonalityMatchingService, PersonalityMatchingProxyService>();
            services.AddTransient<MovieApp.Features.PersonalityMatch.ViewModels.PersonalityMatchViewModel>();
            services.AddTransient<MovieApp.Features.PersonalityMatch.ViewModels.MatchedUserDetailViewModel>();

            // Reels Feed
            services.AddTransient<MovieApp.Logic.Features.ReelsFeed.IClipPlaybackService, MovieApp.Logic.Features.ReelsFeed.ClipPlaybackService>();
            services.AddTransient<MovieApp.Logic.Features.ReelsFeed.IEngagementProfileService, MovieApp.Logic.Features.ReelsFeed.EngagementProfileService>();
            services.AddTransient<MovieApp.Logic.Features.ReelsFeed.IRecommendationService, RecommendationProxyService>();
            services.AddTransient<MovieApp.Logic.Features.ReelsFeed.IReelInteractionService, ReelInteractionProxyService>();
            services.AddTransient<MovieApp.Features.ReelsFeed.ViewModels.ReelsFeedViewModel>();
            services.AddTransient<MovieApp.Features.ReelsFeed.ViewModels.UserProfileViewModel>();

            // Marketplace & Wallet
            services.AddTransient<MarketplaceViewModel>();
            services.AddTransient<SellEquipmentViewModel>();
            services.AddTransient<WalletViewModel>();
            services.AddTransient<FlashSaleViewModel>(sp => new FlashSaleViewModel(DateTime.Now.AddHours(2)));

            var provider = services.BuildServiceProvider();
            Services = provider;
            Ioc.Default.ConfigureServices(provider);
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}

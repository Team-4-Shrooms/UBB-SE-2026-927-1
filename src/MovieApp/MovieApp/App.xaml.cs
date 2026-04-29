using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using MovieApp.Logic.Http;
using MovieApp.DataLayer.Interfaces.Repositories;
using CommunityToolkit.Mvvm.DependencyInjection;
using MovieApp.WebApi.Data;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Services;
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

            var connectionString = "Server=localhost\\SQLEXPRESS;Database=MovieApp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

<<<<<<< HEAD
            services.AddHttpClient<ApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7196/");
            });

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
            services.AddTransient<IScrapeRepository, ScrapeJobProxyRepository>();
            services.AddTransient<ITransactionRepository, TransactionProxyRepository>();
            services.AddTransient<IUserRepository, UserProxyRepository>();
            services.AddTransient<IVideoStorageRepository, VideoStorageProxyRepository>();

            // Logic Services
            services.AddTransient<IEquipmentService, EquipmentService>();

            // Reels Upload
            services.AddTransient<MovieApp.Features.ReelsUpload.Services.IVideoStorageService, MovieApp.Features.ReelsUpload.Services.VideoStorageService>();
            services.AddTransient<MovieApp.Features.ReelsUpload.ViewModels.ReelsUploadViewModel>();

            // Trailer Scraping
            services.AddTransient<MovieApp.Features.TrailerScraping.Services.IYouTubeScraperService>(provider =>
                new MovieApp.Features.TrailerScraping.Services.YouTubeScraperService("YOUR_YOUTUBE_API_KEY"));
            services.AddTransient<MovieApp.Features.TrailerScraping.Services.IVideoDownloadService, MovieApp.Features.TrailerScraping.Services.VideoDownloadService>();
            services.AddTransient<MovieApp.Features.TrailerScraping.Services.IVideoIngestionService, MovieApp.Features.TrailerScraping.Services.VideoIngestionService>();
            services.AddTransient<MovieApp.Features.TrailerScraping.ViewModels.TrailerScrapingViewModel>();

            // Reels Editing
            services.AddTransient<MovieApp.Features.ReelsEditing.Services.IVideoProcessingService, MovieApp.Features.ReelsEditing.Services.VideoProcessingService>();
            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.ReelsEditingViewModel>();
            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.ReelGalleryViewModel>();
            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.MusicSelectionDialogViewModel>();

            // Movie Swipe
            services.AddTransient<MovieApp.Features.MovieSwipe.Services.ISwipeService, MovieApp.Features.MovieSwipe.Services.SwipeService>();
            services.AddTransient<MovieApp.Features.MovieSwipe.Services.IMovieCardFeedService, MovieApp.Features.MovieSwipe.Services.MovieCardFeedService>();
            services.AddTransient<MovieApp.Features.MovieSwipe.ViewModels.MovieSwipeViewModel>();

            // Movie Tournament
            services.AddSingleton<MovieApp.Features.MovieTournament.Services.ITournamentLogicService, MovieApp.Features.MovieTournament.Services.TournamentLogicService>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.MovieTournamentViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentMatchViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentSetupViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentWinnerViewModel>();

            // Personality Match
            services.AddTransient<MovieApp.Features.PersonalityMatch.Services.IPersonalityMatchingService, MovieApp.Features.PersonalityMatch.Services.PersonalityMatchingService>();
            services.AddTransient<MovieApp.Features.PersonalityMatch.ViewModels.PersonalityMatchViewModel>();
            services.AddTransient<MovieApp.Features.PersonalityMatch.ViewModels.MatchedUserDetailViewModel>();

            // Reels Feed
            services.AddTransient<MovieApp.Features.ReelsFeed.Services.IClipPlaybackService, MovieApp.Features.ReelsFeed.Services.ClipPlaybackService>();
            services.AddTransient<MovieApp.Features.ReelsFeed.Services.IEngagementProfileService, MovieApp.Features.ReelsFeed.Services.EngagementProfileService>();
            services.AddTransient<MovieApp.Features.ReelsFeed.Services.IRecommendationService, MovieApp.Features.ReelsFeed.Services.RecommendationService>();
            services.AddTransient<MovieApp.Features.ReelsFeed.Services.IReelInteractionService, MovieApp.Features.ReelsFeed.Services.ReelInteractionService>();
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

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
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IActiveSalesService, ActiveSalesService>();
            services.AddTransient<IReviewService, ReviewService>();
            services.AddTransient<IInventoryService, InventoryService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<IPersonalityMatchService, PersonalityMatchService>();

            // Reels Upload
            services.AddTransient<MovieApp.Logic.Features.ReelsUpload.IVideoStorageService, MovieApp.Logic.Features.ReelsUpload.VideoStorageService>();
            services.AddTransient<MovieApp.Features.ReelsUpload.ViewModels.ReelsUploadViewModel>();

            // Trailer Scraping
            services.AddTransient<MovieApp.Logic.Features.TrailerScraping.IYouTubeScraperService>(provider =>
                new MovieApp.Logic.Features.TrailerScraping.YouTubeScraperService("YOUR_YOUTUBE_API_KEY"));
            services.AddTransient<MovieApp.Logic.Features.TrailerScraping.IVideoDownloadService, MovieApp.Logic.Features.TrailerScraping.VideoDownloadService>();
            services.AddTransient<MovieApp.Logic.Features.TrailerScraping.IVideoIngestionService, MovieApp.Logic.Features.TrailerScraping.VideoIngestionService>();
            services.AddTransient<MovieApp.Features.TrailerScraping.ViewModels.TrailerScrapingViewModel>();

            // Reels Editing
            services.AddTransient<MovieApp.Logic.Features.ReelsEditing.IVideoProcessingService, MovieApp.Logic.Features.ReelsEditing.VideoProcessingService>();
            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.ReelsEditingViewModel>();
            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.ReelGalleryViewModel>();
            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.MusicSelectionDialogViewModel>();

            // Movie Swipe
            services.AddTransient<MovieApp.Logic.Features.MovieSwipe.ISwipeService, MovieApp.Logic.Features.MovieSwipe.SwipeService>();
            services.AddTransient<MovieApp.Logic.Features.MovieSwipe.IMovieCardFeedService, MovieApp.Logic.Features.MovieSwipe.MovieCardFeedService>();
            services.AddTransient<MovieApp.Features.MovieSwipe.ViewModels.MovieSwipeViewModel>();

            // Movie Tournament
            services.AddSingleton<MovieApp.Logic.Features.MovieTournament.ITournamentLogicService, MovieApp.Logic.Features.MovieTournament.TournamentLogicService>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.MovieTournamentViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentMatchViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentSetupViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentWinnerViewModel>();

            // Personality Match
            services.AddTransient<MovieApp.Logic.Features.PersonalityMatch.IPersonalityMatchingService, MovieApp.Logic.Features.PersonalityMatch.PersonalityMatchingService>();
            services.AddTransient<MovieApp.Features.PersonalityMatch.ViewModels.PersonalityMatchViewModel>();
            services.AddTransient<MovieApp.Features.PersonalityMatch.ViewModels.MatchedUserDetailViewModel>();

            // Reels Feed
            services.AddTransient<MovieApp.Logic.Features.ReelsFeed.IClipPlaybackService, MovieApp.Logic.Features.ReelsFeed.ClipPlaybackService>();
            services.AddTransient<MovieApp.Logic.Features.ReelsFeed.IEngagementProfileService, MovieApp.Logic.Features.ReelsFeed.EngagementProfileService>();
            services.AddTransient<MovieApp.Logic.Features.ReelsFeed.IRecommendationService, MovieApp.Logic.Features.ReelsFeed.RecommendationService>();
            services.AddTransient<MovieApp.Logic.Features.ReelsFeed.IReelInteractionService, MovieApp.Logic.Features.ReelsFeed.ReelInteractionService>();
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

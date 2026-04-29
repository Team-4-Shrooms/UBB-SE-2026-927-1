using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using MovieApp.Logic.Http; // Ensures ApiClient is recognized
using MovieApp.DataLayer.Interfaces.Repositories;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
// 🚨 DELETED: using Microsoft.EntityFrameworkCore; - The UI is blind to the DB now!

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MovieApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        public static Window MainWindow => ((App)Current)._window!;

        public App()
        {
            InitializeComponent();
            ConfigureServices();
        }

        private void ConfigureServices()
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
            ServiceCollection services = new ServiceCollection();

            // 🚨 DELETED: AddDbContext and UseSqlServer. The Web API project handles this now.

            // ========================================================================
            // 🟢 SERVICES & VIEWMODELS (These are safe to keep in the UI project)
            // ========================================================================

            services.AddDbContext<MovieApp.WebApi.Data.AppDbContext>(options =>
                options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=MovieApp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"));

            services.AddTransient<MovieApp.DataLayer.Interfaces.IMovieAppDbContext>(provider =>
                provider.GetRequiredService<MovieApp.WebApi.Data.AppDbContext>());

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
            services.AddSingleton<MovieApp.Features.MovieTournament.Services.ITournamentLogicService, MovieApp.Features.MovieTournament.Services.TournamentLogicService>(); services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.MovieTournamentViewModel>();
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


            // ========================================================================
            // 🔴 REPOSITORIES (TASK 10 WARNING)
            // ========================================================================
            // I have commented these out. You cannot register EF Core repositories in 
            // the UI anymore. You must ask your team what the new HTTP clients are named 
            // and swap them in (e.g., MovieApp.Http.MovieRepositoryHttp).

            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IVideoStorageRepository, MovieApp.DataLayer.Repositories.VideoStorageRepository>();
            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IMovieRepository, MovieApp.DataLayer.Repositories.MovieRepository>();
            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IUserRepository, MovieApp.DataLayer.Repositories.UserRepository>();
            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IScrapeRepository, MovieApp.DataLayer.Repositories.ScrapeJobRepository>();
            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IReelRepository, MovieApp.DataLayer.Repositories.ReelRepository>();
            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IAudioLibraryRepository, MovieApp.DataLayer.Repositories.AudioLibraryRepository>();
            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IPreferenceRepository, MovieApp.DataLayer.Repositories.PreferenceRepository>();
            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IMovieTournamentRepository, MovieApp.DataLayer.Repositories.MovieTournamentRepository>();
            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IPersonalityMatchRepository, MovieApp.DataLayer.Repositories.PersonalityMatchRepository>();
            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IProfileRepository, MovieApp.DataLayer.Repositories.ProfileRepository>();
            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IRecommendationRepository, MovieApp.DataLayer.Repositories.RecommendationRepository>();
            services.AddTransient<MovieApp.DataLayer.Interfaces.Repositories.IInteractionRepository, MovieApp.DataLayer.Repositories.InteractionRepository>();

            Ioc.Default.ConfigureServices(services.BuildServiceProvider());
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // 🚨 DELETED: The entire "using (var scope = ... db.Database.EnsureCreated())" block.
            // The Web API is fully responsible for creating and seeding the database now.
            // This also fixes the final two 'var' keywords!

            _window = new MainWindow();
            _window.Activate();
        }
    }
}

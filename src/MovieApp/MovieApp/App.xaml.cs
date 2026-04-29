using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;

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
            ServiceCollection services = new ServiceCollection();

            services.AddDbContext<MovieApp.Logic.Data.AppDbContext>(options =>
            {
                options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=MovieApp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;");
            });
            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IVideoStorageRepository, MovieApp.Logic.Repositories.VideoStorageRepository>();

            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IMovieRepository, MovieApp.Logic.Repositories.MovieRepository>();
            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IUserRepository, MovieApp.Logic.Repositories.UserRepository>();

            services.AddTransient<MovieApp.Features.ReelsUpload.Services.IVideoStorageService, MovieApp.Features.ReelsUpload.Services.VideoStorageService>();
            services.AddTransient<MovieApp.Logic.Interfaces.Services.IMovieService, MovieApp.Logic.Services.MovieService>();

            services.AddTransient<MovieApp.Features.ReelsUpload.ViewModels.ReelsUploadViewModel>();

            services.AddTransient<MovieApp.Features.TrailerScraping.Services.IYouTubeScraperService>(provider =>
                new MovieApp.Features.TrailerScraping.Services.YouTubeScraperService("YOUR_YOUTUBE_API_KEY"));

            services.AddTransient<MovieApp.Features.TrailerScraping.Services.IVideoDownloadService,
                MovieApp.Features.TrailerScraping.Services.VideoDownloadService>();

            services.AddTransient<MovieApp.Features.TrailerScraping.Services.IVideoIngestionService,
                MovieApp.Features.TrailerScraping.Services.VideoIngestionService>();

            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IScrapeRepository,
                MovieApp.Logic.Repositories.ScrapeJobRepository>();

            services.AddTransient<MovieApp.Features.TrailerScraping.ViewModels.TrailerScrapingViewModel>();

            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IReelRepository,
                MovieApp.Logic.Repositories.ReelRepository>();

            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IAudioLibraryRepository,
                MovieApp.Logic.Repositories.AudioLibraryRepository>();

            services.AddTransient<MovieApp.Features.ReelsEditing.Services.IVideoProcessingService,
                MovieApp.Features.ReelsEditing.Services.VideoProcessingService>();

            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.ReelsEditingViewModel>();
            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.ReelGalleryViewModel>();
            services.AddTransient<MovieApp.Features.ReelsEditing.ViewModels.MusicSelectionDialogViewModel>();

            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IPreferenceRepository,
                MovieApp.Logic.Repositories.PreferenceRepository>();

            services.AddTransient<MovieApp.Features.MovieSwipe.Services.ISwipeService,
                MovieApp.Features.MovieSwipe.Services.SwipeService>();
            services.AddTransient<MovieApp.Features.MovieSwipe.Services.IMovieCardFeedService,
                MovieApp.Features.MovieSwipe.Services.MovieCardFeedService>();

            services.AddTransient<MovieApp.Features.MovieSwipe.ViewModels.MovieSwipeViewModel>();

            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IMovieTournamentRepository,
                MovieApp.Logic.Repositories.MovieTournamentRepository>();

            services.AddTransient<MovieApp.Features.MovieTournament.Services.ITournamentLogicService,
                MovieApp.Features.MovieTournament.Services.TournamentLogicService>();

            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.MovieTournamentViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentMatchViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentSetupViewModel>();
            services.AddTransient<MovieApp.Features.MovieTournament.ViewModels.TournamentWinnerViewModel>();

            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IPersonalityMatchRepository,
                MovieApp.Logic.Repositories.PersonalityMatchRepository>();

            services.AddTransient<MovieApp.Features.PersonalityMatch.Services.IPersonalityMatchingService,
                MovieApp.Features.PersonalityMatch.Services.PersonalityMatchingService>();

            services.AddTransient<MovieApp.Features.PersonalityMatch.ViewModels.PersonalityMatchViewModel>();
            services.AddTransient<MovieApp.Features.PersonalityMatch.ViewModels.MatchedUserDetailViewModel>();

            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IProfileRepository,
                MovieApp.Logic.Repositories.ProfileRepository>();
            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IRecommendationRepository,
                MovieApp.Logic.Repositories.RecommendationRepository>();
            services.AddTransient<MovieApp.Logic.Interfaces.Repositories.IInteractionRepository,
                MovieApp.Logic.Repositories.InteractionRepository>();

            services.AddTransient<MovieApp.Features.ReelsFeed.Services.IClipPlaybackService,
                MovieApp.Features.ReelsFeed.Services.ClipPlaybackService>();
            services.AddTransient<MovieApp.Features.ReelsFeed.Services.IEngagementProfileService,
                MovieApp.Features.ReelsFeed.Services.EngagementProfileService>();
            services.AddTransient<MovieApp.Features.ReelsFeed.Services.IRecommendationService,
                MovieApp.Features.ReelsFeed.Services.RecommendationService>();
            services.AddTransient<MovieApp.Features.ReelsFeed.Services.IReelInteractionService,
                MovieApp.Features.ReelsFeed.Services.ReelInteractionService>();

            services.AddTransient<MovieApp.Features.ReelsFeed.ViewModels.ReelsFeedViewModel>();
            services.AddTransient<MovieApp.Features.ReelsFeed.ViewModels.UserProfileViewModel>();

            Ioc.Default.ConfigureServices(services.BuildServiceProvider());
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            using (var scope = Ioc.Default.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MovieApp.Logic.Data.AppDbContext>();
                db.Database.EnsureCreated(); // Forces the DB to build if it doesn't exist!
            }
            _window = new MainWindow();
            _window.Activate();
        }
    }
}

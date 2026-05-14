using MovieApp.Logic.Features.MovieSwipe;
using MovieApp.Logic.Features.MovieTournament;
using MovieApp.Logic.Features.PersonalityMatch;
using MovieApp.Logic.Features.ReelsEditing;
using MovieApp.Logic.Features.ReelsFeed;
using MovieApp.Logic.Features.ReelsUpload;
using MovieApp.Logic.Features.TrailerScraping;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Proxy;
using MovieApp.Proxy.Services;
using MovieApp.Web.Auth;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

// HTTP client for auto-login and for ApiClient
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ApiClient>(httpClient =>
    httpClient.BaseAddress = new Uri(config["WebApi:BaseUrl"]!));

// JWT token store — single instance satisfying both IAuthTokenProvider and ICurrentUserService
builder.Services.AddSingleton<JwtTokenStore>();
builder.Services.AddSingleton<IAuthTokenProvider>(serviceProvider => serviceProvider.GetRequiredService<JwtTokenStore>());
builder.Services.AddSingleton<ICurrentUserService>(serviceProvider => serviceProvider.GetRequiredService<JwtTokenStore>());

// Auto-login on startup
builder.Services.AddHostedService<JwtAutoLoginService>();

// Proxy services
builder.Services.AddTransient<IReelService, ReelProxyService>();
builder.Services.AddTransient<IAudioLibraryService, AudioLibraryProxyService>();
builder.Services.AddTransient<IMovieService, MovieProxyService>();
builder.Services.AddTransient<IEquipmentService, EquipmentProxyService>();
builder.Services.AddTransient<IEventService, EventProxyService>();
builder.Services.AddTransient<IInventoryService, InventoryProxyService>();
builder.Services.AddTransient<IReviewService, ReviewProxyService>();
builder.Services.AddTransient<IActiveSalesService, ActiveSalesProxyService>();
builder.Services.AddTransient<IProfileService, ProfileProxyService>();
builder.Services.AddTransient<IPersonalityMatchService, PersonalityMatchProxyService>();
builder.Services.AddTransient<IMovieCardFeedService, MovieCardFeedProxyService>();
builder.Services.AddTransient<ISwipeService, SwipeProxyService>();
builder.Services.AddTransient<IPersonalityMatchingService, PersonalityMatchingProxyService>();
builder.Services.AddTransient<IRecommendationService, RecommendationProxyService>();
builder.Services.AddTransient<IReelInteractionService, ReelInteractionProxyService>();
builder.Services.AddTransient<IVideoProcessingService, VideoProcessingProxyService>();
builder.Services.AddTransient<IVideoStorageService, VideoStorageProxyService>();
builder.Services.AddTransient<IVideoIngestionService, VideoIngestionProxyService>();
builder.Services.AddSingleton<ITournamentLogicService, TournamentLogicProxyService>();
builder.Services.AddTransient<IMovieTournamentService, MovieTournamentProxyService>();

// Cache and session
builder.Services.AddMemoryCache();
builder.Services.AddSession();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

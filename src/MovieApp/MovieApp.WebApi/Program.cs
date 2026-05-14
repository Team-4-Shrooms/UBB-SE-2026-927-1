using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using MovieApp.WebApi.Auth;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Features.MovieSwipe;
using MovieApp.Logic.Features.MovieTournament;
using MovieApp.Logic.Features.PersonalityMatch;
using MovieApp.Logic.Features.ReelsEditing;
using MovieApp.Logic.Features.ReelsFeed;
using MovieApp.Logic.Features.ReelsUpload;
using MovieApp.Logic.Features.TrailerScraping;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Services;
using MovieApp.WebApi.Data;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, WebApiCurrentUserService>();
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IMovieAppDbContext>(serviceProvider => serviceProvider.GetRequiredService<AppDbContext>());

// Repositories — concrete types registered first so controllers can inject them directly;
// interface registrations delegate to the same scoped instance.
builder.Services.AddScoped<ActiveSalesRepository>();
builder.Services.AddScoped<IActiveSalesRepository>(serviceProvider => serviceProvider.GetRequiredService<ActiveSalesRepository>());

builder.Services.AddScoped<AudioLibraryRepository>();
builder.Services.AddScoped<IAudioLibraryRepository>(serviceProvider => serviceProvider.GetRequiredService<AudioLibraryRepository>());

builder.Services.AddScoped<EquipmentRepository>();
builder.Services.AddScoped<IEquipmentRepository>(serviceProvider => serviceProvider.GetRequiredService<EquipmentRepository>());

builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<IEventRepository>(serviceProvider => serviceProvider.GetRequiredService<EventRepository>());

builder.Services.AddScoped<InteractionRepository>();
builder.Services.AddScoped<IInteractionRepository>(serviceProvider => serviceProvider.GetRequiredService<InteractionRepository>());

builder.Services.AddScoped<InventoryRepository>();
builder.Services.AddScoped<IInventoryRepository>(serviceProvider => serviceProvider.GetRequiredService<InventoryRepository>());

builder.Services.AddScoped<MovieRepository>();
builder.Services.AddScoped<IMovieRepository>(serviceProvider => serviceProvider.GetRequiredService<MovieRepository>());

builder.Services.AddScoped<MovieTournamentRepository>();
builder.Services.AddScoped<IMovieTournamentRepository>(serviceProvider => serviceProvider.GetRequiredService<MovieTournamentRepository>());

builder.Services.AddScoped<PersonalityMatchRepository>();
builder.Services.AddScoped<IPersonalityMatchRepository>(serviceProvider => serviceProvider.GetRequiredService<PersonalityMatchRepository>());

builder.Services.AddScoped<PreferenceRepository>();
builder.Services.AddScoped<IPreferenceRepository>(serviceProvider => serviceProvider.GetRequiredService<PreferenceRepository>());

builder.Services.AddScoped<ProfileRepository>();
builder.Services.AddScoped<IProfileRepository>(serviceProvider => serviceProvider.GetRequiredService<ProfileRepository>());

builder.Services.AddScoped<RecommendationRepository>();
builder.Services.AddScoped<IRecommendationRepository>(serviceProvider => serviceProvider.GetRequiredService<RecommendationRepository>());

builder.Services.AddScoped<ReelRepository>();
builder.Services.AddScoped<IReelRepository>(serviceProvider => serviceProvider.GetRequiredService<ReelRepository>());

builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<IReviewRepository>(serviceProvider => serviceProvider.GetRequiredService<ReviewRepository>());

builder.Services.AddScoped<ScrapeJobRepository>();
builder.Services.AddScoped<IScrapeJobRepository>(serviceProvider => serviceProvider.GetRequiredService<ScrapeJobRepository>());

builder.Services.AddScoped<TransactionRepository>();
builder.Services.AddScoped<ITransactionRepository>(serviceProvider => serviceProvider.GetRequiredService<TransactionRepository>());

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IUserRepository>(serviceProvider => serviceProvider.GetRequiredService<UserRepository>());

builder.Services.AddScoped<VideoStorageRepository>();
builder.Services.AddScoped<IVideoStorageRepository>(serviceProvider => serviceProvider.GetRequiredService<VideoStorageRepository>());

// Core services
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IActiveSalesService, ActiveSalesService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IPersonalityMatchService, PersonalityMatchService>();
builder.Services.AddScoped<IPreferenceService, PreferenceService>();
builder.Services.AddScoped<IMovieTournamentService, MovieTournamentService>();
builder.Services.AddScoped<IAudioLibraryService, AudioLibraryService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IScrapeJobService, ScrapeJobService>();
builder.Services.AddScoped<IReelService, ReelService>();

// Feature services
builder.Services.AddScoped<IMovieCardFeedService, MovieCardFeedService>();
builder.Services.AddScoped<ISwipeService, SwipeService>();
builder.Services.AddScoped<IPersonalityMatchingService, PersonalityMatchingService>();
builder.Services.AddScoped<IReelInteractionService, ReelInteractionService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IVideoProcessingService, VideoProcessingService>();
string videoUploadDir = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads", "videos");
Directory.CreateDirectory(videoUploadDir);
builder.Services.AddScoped<IVideoStorageService>(serviceProvider =>
    new VideoStorageService(
        serviceProvider.GetRequiredService<IVideoStorageRepository>(),
        serviceProvider.GetRequiredService<IReelRepository>(),
        videoUploadDir,
        "/uploads/videos"));
builder.Services.AddScoped<IVideoIngestionService, VideoIngestionService>();
builder.Services.AddSingleton<ITournamentLogicService, TournamentLogicService>();

// Infrastructure
builder.Services.AddSingleton<IVideoDownloadService, VideoDownloadService>();
builder.Services.AddTransient<IYouTubeScraperService>(_ =>
    new YouTubeScraperService(config["YouTube:ApiKey"] ?? string.Empty));
builder.Services.AddTransient<IWebScraperService>(sp =>
    (IWebScraperService)sp.GetRequiredService<IYouTubeScraperService>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MovieApp WebApi",
        Version = "v1",
        Description = "HTTP API for the MovieApp application."
    });

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (context.Database.IsSqlServer())
    {
        try
        {
            // Use a short timeout so a locked __EFMigrationsHistory table (common after a
            // previous crash) fails fast instead of blocking startup for 30+ seconds.
            context.Database.SetCommandTimeout(TimeSpan.FromSeconds(20));
            await context.Database.MigrateAsync();
        }
        catch (Exception migEx)
        {
            Console.Error.WriteLine($"[Startup] MigrateAsync skipped (non-fatal): {migEx.Message}");
        }
        finally
        {
            // Restore normal timeout for seeding queries.
            context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));
        }
    }

    try
    {
        DataSeeder seeder = new DataSeeder(context);
        await seeder.SeedAsync();

        // Replace placeholder hashes with real BCrypt hashes on first run.
        foreach (var user in context.Users.Where(user => user.PasswordHash.StartsWith("placeholder_")))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(config["Auth:SeedPassword"] ?? "password123");
        }

        await context.SaveChangesAsync();
    }
    catch (Exception seedEx)
    {
        Console.Error.WriteLine($"[Startup] Seeding warning (non-fatal): {seedEx.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieApp WebApi v1");
        options.DocumentTitle = "MovieApp WebApi";
    });
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

string uploadsPhysicalPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads");
Directory.CreateDirectory(uploadsPhysicalPath);
FileExtensionContentTypeProvider contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".tmp"] = "video/mp4";
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPhysicalPath),
    RequestPath = "/uploads",
    ContentTypeProvider = contentTypeProvider,
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Enables WebApplicationFactory discovery for integration tests.
/// </summary>
public partial class Program { }

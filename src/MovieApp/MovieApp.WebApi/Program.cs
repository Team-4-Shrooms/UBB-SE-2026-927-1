using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IMovieAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Repositories
builder.Services.AddScoped<IActiveSalesRepository, ActiveSalesRepository>();
builder.Services.AddScoped<IAudioLibraryRepository, AudioLibraryRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IInteractionRepository, InteractionRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieTournamentRepository, MovieTournamentRepository>();
builder.Services.AddScoped<IPersonalityMatchRepository, PersonalityMatchRepository>();
builder.Services.AddScoped<IPreferenceRepository, PreferenceRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();
builder.Services.AddScoped<IReelRepository, ReelRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IScrapeRepository, ScrapeJobRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVideoStorageRepository, VideoStorageRepository>();

// Core services
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IActiveSalesService, ActiveSalesService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IPersonalityMatchService, PersonalityMatchService>();

// Feature services
builder.Services.AddScoped<IMovieCardFeedService, MovieCardFeedService>();
builder.Services.AddScoped<ISwipeService, SwipeService>();
builder.Services.AddScoped<IPersonalityMatchingService, PersonalityMatchingService>();
builder.Services.AddScoped<IReelInteractionService, ReelInteractionService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IVideoProcessingService, VideoProcessingService>();
builder.Services.AddScoped<IVideoStorageService, VideoStorageService>();
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

builder.WebHost.UseUrls("http://localhost:4544");

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (context.Database.IsSqlServer())
    {
        await context.Database.MigrateAsync();
    }

    DataSeeder seeder = new DataSeeder(context);
    await seeder.SeedAsync();

    // Replace placeholder hashes with real BCrypt hashes on first run.
    foreach (var user in context.Users.Where(u => u.PasswordHash.StartsWith("placeholder_")))
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(config["Auth:SeedPassword"] ?? "password123");
    }

    await context.SaveChangesAsync();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Enables WebApplicationFactory discovery for integration tests.
/// </summary>
public partial class Program { }

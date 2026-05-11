using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.Auth;
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
builder.Services.AddScoped<ActiveSalesRepository>();
builder.Services.AddScoped<AudioLibraryRepository>();
builder.Services.AddScoped<InteractionRepository>();
builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<EquipmentRepository>();
builder.Services.AddScoped<MovieRepository>();
builder.Services.AddScoped<InventoryRepository>();
builder.Services.AddScoped<MovieTournamentRepository>();
builder.Services.AddScoped<ProfileRepository>();
builder.Services.AddScoped<PreferenceRepository>();
builder.Services.AddScoped<PersonalityMatchRepository>();
builder.Services.AddScoped<RecommendationRepository>();
builder.Services.AddScoped<ReelRepository>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<VideoStorageRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<TransactionRepository>();
builder.Services.AddScoped<ScrapeJobRepository>();
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

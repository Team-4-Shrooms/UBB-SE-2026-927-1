using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Repositories;
using MovieApp.WebApi.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Enables WebApplicationFactory discovery for integration tests.
/// </summary>
public partial class Program { }

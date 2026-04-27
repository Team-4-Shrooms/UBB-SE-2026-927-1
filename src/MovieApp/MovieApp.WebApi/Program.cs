using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.DataLayer.Repositories;
using MovieApp.WebApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<WebAPIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IMovieAppDbContext>(sp => sp.GetRequiredService<WebAPIDbContext>());
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
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    WebAPIDbContext context = scope.ServiceProvider.GetRequiredService<WebAPIDbContext>();
    await context.Database.MigrateAsync();

    DataSeeder seeder = new DataSeeder(context);
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using FootballyAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Entity Framework with SQLite
builder.Services.AddDbContext<FootballyDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                     "Data Source=footbally.db"));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Footbally API", 
        Version = "v1",
        Description = "A comprehensive football player statistics API"
    });
});

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure to run on port 8001
app.Urls.Add("http://0.0.0.0:8001");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Footbally API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app root
    });
}

// Enable CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Create database and apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FootballyDbContext>();
    try
    {
        context.Database.EnsureCreated();
        
        // Seed sample data if database is empty
        if (!context.Teams.Any())
        {
            var sampleTeams = new[]
            {
                new FootballyAPI.Models.Team
                {
                    Name = "Manchester United",
                    ShortName = "MUN",
                    City = "Manchester",
                    Country = "England",
                    Stadium = "Old Trafford",
                    Founded = 1878,
                    PrimaryColor = "#DA020E",
                    SecondaryColor = "#FFFFFF",
                    Manager = "Erik ten Hag",
                    League = "Premier League"
                },
                new FootballyAPI.Models.Team
                {
                    Name = "Real Madrid CF",
                    ShortName = "RMA",
                    City = "Madrid",
                    Country = "Spain",
                    Stadium = "Santiago Bernabéu",
                    Founded = 1902,
                    PrimaryColor = "#FFFFFF",
                    SecondaryColor = "#FEBE10",
                    Manager = "Carlo Ancelotti",
                    League = "La Liga"
                },
                new FootballyAPI.Models.Team
                {
                    Name = "FC Barcelona",
                    ShortName = "BAR",
                    City = "Barcelona",
                    Country = "Spain",
                    Stadium = "Camp Nou",
                    Founded = 1899,
                    PrimaryColor = "#A50044",
                    SecondaryColor = "#004D98",
                    Manager = "Xavi Hernández",
                    League = "La Liga"
                }
            };
            
            context.Teams.AddRange(sampleTeams);
            context.SaveChanges();
            
            // Add sample players
            var samplePlayers = new[]
            {
                new FootballyAPI.Models.Player
                {
                    Name = "Marcus Rashford",
                    Position = "Left Winger",
                    Age = 27,
                    Nationality = "England",
                    TeamId = sampleTeams[0].Id,
                    Height = 180,
                    Weight = 70,
                    PreferredFoot = "Right",
                    JerseyNumber = 10
                },
                new FootballyAPI.Models.Player
                {
                    Name = "Karim Benzema",
                    Position = "Center Forward",
                    Age = 36,
                    Nationality = "France",
                    TeamId = sampleTeams[1].Id,
                    Height = 185,
                    Weight = 81,
                    PreferredFoot = "Right",
                    JerseyNumber = 9
                },
                new FootballyAPI.Models.Player
                {
                    Name = "Robert Lewandowski",
                    Position = "Center Forward",
                    Age = 35,
                    Nationality = "Poland",
                    TeamId = sampleTeams[2].Id,
                    Height = 185,
                    Weight = 81,
                    PreferredFoot = "Right",
                    JerseyNumber = 9
                }
            };
            
            context.Players.AddRange(samplePlayers);
            context.SaveChanges();
            
            // Add sample statistics
            var sampleStats = new[]
            {
                new FootballyAPI.Models.PlayerStatistics
                {
                    PlayerId = samplePlayers[0].Id,
                    Season = "2024-25",
                    GamesPlayed = 25,
                    GamesStarted = 22,
                    MinutesPlayed = 2100,
                    Goals = 12,
                    Assists = 8,
                    YellowCards = 3,
                    RedCards = 0,
                    Tackles = 45,
                    Interceptions = 25,
                    ShotsTotal = 85,
                    ShotsOnTarget = 42,
                    PassesAttempted = 1200,
                    PassesCompleted = 980,
                    KeyPasses = 65,
                    AverageRating = 7.5,
                    Wins = 15,
                    Draws = 6,
                    Losses = 4
                },
                new FootballyAPI.Models.PlayerStatistics
                {
                    PlayerId = samplePlayers[1].Id,
                    Season = "2024-25",
                    GamesPlayed = 28,
                    GamesStarted = 26,
                    MinutesPlayed = 2400,
                    Goals = 18,
                    Assists = 12,
                    YellowCards = 2,
                    RedCards = 0,
                    Tackles = 25,
                    Interceptions = 15,
                    ShotsTotal = 95,
                    ShotsOnTarget = 58,
                    PassesAttempted = 1400,
                    PassesCompleted = 1150,
                    KeyPasses = 78,
                    AverageRating = 8.2,
                    Wins = 20,
                    Draws = 5,
                    Losses = 3
                }
            };
            
            context.PlayerStatistics.AddRange(sampleStats);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

app.Run();
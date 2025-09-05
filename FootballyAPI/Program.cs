using Microsoft.EntityFrameworkCore;
using FootballyAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Entity Framework with SQLite
builder.Services.AddDbContext<FootballyDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                     "Data Source=footbally.db"));

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<FootballyDbContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey") ?? "DefaultSecretKeyForFootballyAPI123456789";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.GetValue<string>("Issuer") ?? "FootballyAPI",
        ValidAudience = jwtSettings.GetValue<string>("Audience") ?? "FootballyAPI",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

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
        Description = "A comprehensive football management API with tournaments, matches, and player analysis"
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Create database and apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FootballyDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    try
    {
        context.Database.EnsureCreated();
        
        // Create roles
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        if (!await roleManager.RoleExistsAsync("Referee"))
            await roleManager.CreateAsync(new IdentityRole("Referee"));
        if (!await roleManager.RoleExistsAsync("Viewer"))
            await roleManager.CreateAsync(new IdentityRole("Viewer"));
        
        // Create default admin user
        if (await userManager.FindByEmailAsync("admin@footbally.com") == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = "admin@footbally.com",
                Email = "admin@footbally.com",
                EmailConfirmed = true
            };
            
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        
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
            await context.SaveChangesAsync();

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
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

app.Run();
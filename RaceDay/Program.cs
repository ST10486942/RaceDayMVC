
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RaceDayAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. DATABASE CONFIGURATION
// ==========================================

var connectionString =
    builder.Configuration.GetConnectionString("RaceDayConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'RaceDayConnection' was not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ==========================================
// 2. JWT CONFIGURATION
// ==========================================

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT signing key is missing.");

if (jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "The JWT signing key must contain at least 32 characters.");
}

// ==========================================
// 3. API SERVICES
// ==========================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// ==========================================
// 4. SWAGGER CONFIGURATION
// ==========================================

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RaceDay API",
        Version = "v1",
        Description = "RaceDay RESTful API for organisers and participants."
    });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

// ==========================================
// 5. AUTHENTICATION
// ==========================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.FromMinutes(1)
            };
    });

// ==========================================
// 6. AUTHORISATION
// ==========================================

builder.Services.AddAuthorization();

// ==========================================
// 7. BUILD APPLICATION
// ==========================================

var app = builder.Build();

// ==========================================
// 8. CREATE LOCAL IMAGE DIRECTORIES
// ==========================================

// Images will be stored locally instead of Azure Blob Storage.

var webRootPath = app.Environment.WebRootPath;

if (string.IsNullOrWhiteSpace(webRootPath))
{
    webRootPath = Path.Combine(
        app.Environment.ContentRootPath,
        "wwwroot");
}

Directory.CreateDirectory(webRootPath);

var uploadsFolder = Path.Combine(
    webRootPath,
    "uploads");

var profilesFolder = Path.Combine(
    uploadsFolder,
    "profiles");

var eventsFolder = Path.Combine(
    uploadsFolder,
    "events");

Directory.CreateDirectory(uploadsFolder);
Directory.CreateDirectory(profilesFolder);
Directory.CreateDirectory(eventsFolder);

// ==========================================
// 9. SWAGGER MIDDLEWARE
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ==========================================
// 10. HTTPS
// ==========================================

app.UseHttpsRedirection();

// ==========================================
// 11. STATIC FILES (WWWROOT)
// ==========================================

// Allows browsers to access uploaded images.
// Example:
// https://localhost:7001/uploads/profiles/photo.jpg

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(webRootPath)
});

// ==========================================
// 12. AUTHENTICATION AND AUTHORISATION
// ==========================================

app.UseAuthentication();

app.UseAuthorization();

// ==========================================
// 13. MAP API CONTROLLERS
// ==========================================

app.MapControllers();

// ==========================================
// 14. START APPLICATION
// ==========================================

app.Run();

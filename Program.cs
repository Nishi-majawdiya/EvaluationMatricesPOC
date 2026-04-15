using EvaluationMatricesPOC.Models;
using EvaluationMatricesPOC.Services;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// 🔥 SERILOG CONFIGURATION (VERY IMPORTANT)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("🚀 Application Starting...");

    var builder = WebApplication.CreateBuilder(args);

    // 🔥 USE SERILOG
    builder.Host.UseSerilog();

    // MVC
    builder.Services.AddControllersWithViews();

    // DB
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()
        ));

    // Services
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddSingleton<IRainfallService, RainfallService>();
    builder.Services.AddSingleton<ICaloriesService, CaloriesService>();
    builder.Services.AddSingleton<ILaptopService, LaptopService>();

    // JWT
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration");
            var key = Encoding.UTF8.GetBytes(jwtKey);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    // 🌧 AUTO TRAIN: Rainfall
    var rainfallPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModels", "rainfallModel.zip");

    if (!File.Exists(rainfallPath))
    {
        Log.Information("🌧 Training Rainfall Model...");
        MLTrainer.TrainRainfallModel();
    }

    // 🔥 AUTO TRAIN: Calories
    var caloriesPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModels", "caloriesModel.zip");

    if (!File.Exists(caloriesPath))
    {
        Log.Information("⚡ Training Calories Model...");
        MLTrainerCalories.Train();
    }

    // 💻 AUTO TRAIN: Laptop
    var laptopPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModels", "laptopModel.zip");

    if (!File.Exists(laptopPath))
    {
        Log.Information("💻 Training Laptop Model...");
        MLTrainerLaptop.Train();
    }

    // Pipeline
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    // 🔥 GLOBAL ERROR LOG
    Log.Fatal(ex, "❌ Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}
using System.Text.Json.Serialization;
using Docket.Security.Extensions;
using DotNetEnv;
using IdentityService.Configuration;
using IdentityService.Data;
using IdentityService.Middleware;
using IdentityService.Repositories.Implementations;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Implementations;
using IdentityService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ── Controllers & JSON ───────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ── Configuration & Security ─────────────────────────────────────────────────
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.AddDocketSecurity(builder.Configuration);

// ── Database ─────────────────────────────────────────────────────────────────
var dbSettings = builder.Configuration
    .GetSection(DatabaseSettings.SectionName)
    .Get<DatabaseSettings>()
    ?? throw new InvalidOperationException("Database configuration is missing.");

var connectionString = new MySqlConnectionStringBuilder
{
    Server   = dbSettings.Host,
    Port     = (uint)dbSettings.Port,
    Database = dbSettings.Name,
    UserID   = dbSettings.Username,
    Password = dbSettings.Password
}.ConnectionString;

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 4, 0))));

// ── Repositories ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IDriverProfileRepository, DriverProfileRepository>();

// ── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDriverService, DriverService>();

var app = builder.Build();

// ── Middleware Pipeline ──────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

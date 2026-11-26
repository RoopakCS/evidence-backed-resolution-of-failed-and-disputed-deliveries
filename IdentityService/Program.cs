using DotNetEnv;
using IdentityService.Configuration;
using IdentityService.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.MapGet("/", () => "Identity Service");

app.Run();

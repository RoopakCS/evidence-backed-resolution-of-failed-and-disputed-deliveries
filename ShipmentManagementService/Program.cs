using System.Text.Json.Serialization;
using Docket.Security.Extensions;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using ShipmentManagementService.Configuration;
using ShipmentManagementService.Data;
using ShipmentManagementService.Middleware;
using ShipmentManagementService.Repositories.Implementations;
using ShipmentManagementService.Repositories.Interfaces;
using ShipmentManagementService.Services.Implementations;
using ShipmentManagementService.Services.Interfaces;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ── Controllers & JSON ───────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ── Security ─────────────────────────────────────────────────────────────────
builder.Services.AddDocketSecurity(builder.Configuration);

// ── Database ─────────────────────────────────────────────────────────────────
var dbSettings = builder.Configuration
    .GetSection(DatabaseSettings.SectionName)
    .Get<DatabaseSettings>()
    ?? throw new InvalidOperationException("Database configuration is missing.");

var connectionString = new MySqlConnectionStringBuilder
{
    Server = dbSettings.Host,
    Port = (uint)dbSettings.Port,
    Database = dbSettings.Name,
    UserID = dbSettings.Username,
    Password = dbSettings.Password
}.ConnectionString;

builder.Services.AddDbContext<ShipmentDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 4, 0))));

// ── Repositories ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();

// ── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IInstructionService, InstructionService>();
builder.Services.AddScoped<IRescheduleService, RescheduleService>();
builder.Services.AddScoped<IReturnService, ReturnService>();

var app = builder.Build();

// ── Middleware Pipeline ──────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

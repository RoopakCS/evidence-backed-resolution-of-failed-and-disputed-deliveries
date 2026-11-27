using System.Text.Json.Serialization;
using Amazon;
using Amazon.S3;
using DeliveryAttemptService.Configuration;
using DeliveryAttemptService.Data;
using DeliveryAttemptService.Middleware;
using DeliveryAttemptService.Repositories.Implementations;
using DeliveryAttemptService.Repositories.Interfaces;
using DeliveryAttemptService.Services.Implementations;
using DeliveryAttemptService.Services.Interfaces;
using Docket.Security.Extensions;
using DotNetEnv;
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
var evidenceStorageSettings = builder.Configuration
    .GetSection(EvidenceStorageSettings.SectionName)
    .Get<EvidenceStorageSettings>()
    ?? throw new InvalidOperationException("Evidence storage configuration is missing.");

builder.Services.Configure<EvidenceStorageSettings>(
    builder.Configuration.GetSection(EvidenceStorageSettings.SectionName));
builder.Services.AddSingleton<IAmazonS3>(
    _ => new AmazonS3Client(RegionEndpoint.GetBySystemName(evidenceStorageSettings.Region)));

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

builder.Services.AddDbContext<DeliveryAttemptDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 4, 0))));

// ── Repositories ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IDeliveryAttemptRepository, DeliveryAttemptRepository>();

// ── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IInstructionDecisionService, InstructionDecisionService>();
builder.Services.AddScoped<IAttemptService, AttemptService>();
builder.Services.AddScoped<IEvidenceService, EvidenceService>();
builder.Services.AddScoped<IOfflineSyncService, OfflineSyncService>();

var app = builder.Build();

// ── Middleware Pipeline ──────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

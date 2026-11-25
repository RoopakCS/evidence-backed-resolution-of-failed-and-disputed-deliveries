using DotNetEnv;
using Docket.Security.Extensions;
using ChatbotService.Configuration;
using ChatbotService.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDocketSecurity(builder.Configuration);

var databaseSettings = builder.Configuration
    .GetSection("Database")
    .Get<DatabaseSettings>()
    ?? throw new InvalidOperationException(
        "Database configuration is missing.");

var connectionStringBuilder = new MySqlConnectionStringBuilder
{
    Server = databaseSettings.Host,
    Port = (uint)databaseSettings.Port,
    Database = databaseSettings.Name,
    UserID = databaseSettings.Username,
    Password = databaseSettings.Password
};

var connectionString = connectionStringBuilder.ConnectionString;

builder.Services.AddDbContext<ChatbotDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 4, 0))));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
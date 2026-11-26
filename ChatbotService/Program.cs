using ChatbotService.AI;
using ChatbotService.Configuration;
using ChatbotService.Data;
using ChatbotService.Repositories.Implementations;
using ChatbotService.Repositories.Interfaces;
using ChatbotService.Services.Implementations;
using ChatbotService.Services.Interfaces;
using Docket.Security.Extensions;
using DotNetEnv;
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

builder.Services.AddScoped<IConversationRepository, ConversationRepository>();

builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddSingleton<PromptLoader>();

builder.Services.AddHttpClient<GroqClient>();

builder.Services.Configure<ChatbotSettings>(
    builder.Configuration.GetSection("Chatbot"));

builder.Services.Configure<GroqSettings>(
    builder.Configuration.GetSection("Groq"));

builder.Services.PostConfigure<GroqSettings>(options =>
{
    options.ApiKey = builder.Configuration["GROQ_API_KEY"]
        ?? options.ApiKey;
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
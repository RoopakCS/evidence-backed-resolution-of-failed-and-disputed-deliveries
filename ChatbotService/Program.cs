using DotNetEnv;
using Docket.Security.Extensions;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration["Jwt:SecretKey"] =
    Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

builder.Configuration["Jwt:Issuer"] =
    Environment.GetEnvironmentVariable("JWT_ISSUER");

builder.Configuration["Jwt:Audience"] =
    Environment.GetEnvironmentVariable("JWT_AUDIENCE");

builder.Services.AddControllers();

builder.Services.AddDocketSecurity(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
using System.Text;
using Docket.Security.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Docket.Security.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection AddDocketSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration is missing.");

        if (string.IsNullOrWhiteSpace(jwt.SecretKey))
            throw new InvalidOperationException(
                "JWT secret key is missing.");

        if (string.IsNullOrWhiteSpace(jwt.Issuer))
            throw new InvalidOperationException(
                "JWT issuer is missing.");

        if (string.IsNullOrWhiteSpace(jwt.Audience))
            throw new InvalidOperationException(
                "JWT audience is missing.");

        var key = Encoding.UTF8.GetBytes(jwt.SecretKey);

        services.AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,

                        ValidateLifetime = true,
                        RequireExpirationTime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(key),

                        ClockSkew = TimeSpan.FromSeconds(30),

                        ValidAlgorithms = new[]
                        {
                            SecurityAlgorithms.HmacSha256
                        },

                        NameClaimType = "sub",
                        RoleClaimType = "role"
                    };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AuthenticatedUser", policy =>
                policy.RequireAuthenticatedUser());
        });

        return services;
    }
}
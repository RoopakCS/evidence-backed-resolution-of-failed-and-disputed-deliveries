using System.Security.Claims;
using Docket.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ShipmentManagementService.Exceptions;

namespace ShipmentManagementService.Controllers;

internal static class ControllerExtensions
{
    public static Guid GetRequiredUserId(this ClaimsPrincipal user)
    {
        var raw = user.GetUserId();
        if (!Guid.TryParse(raw, out var userId))
            throw new ServiceException(StatusCodes.Status401Unauthorized, "Invalid or missing user identifier.");

        return userId;
    }

    public static string GetRequiredRole(this ClaimsPrincipal user)
    {
        return user.GetRole()
            ?? throw new ServiceException(StatusCodes.Status401Unauthorized, "Invalid or missing user role.");
    }

    public static string? GetIdempotencyKey(this HttpRequest request)
    {
        if (request.Headers.TryGetValue("Idempotency-Key", out var values))
        {
            var key = values.FirstOrDefault()?.Trim();
            return string.IsNullOrEmpty(key) ? null : key;
        }

        return null;
    }
}

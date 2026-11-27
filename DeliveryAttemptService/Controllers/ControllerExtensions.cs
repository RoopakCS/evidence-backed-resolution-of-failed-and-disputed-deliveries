using System.Security.Claims;
using DeliveryAttemptService.Exceptions;
using Docket.Security.Claims;

namespace DeliveryAttemptService.Controllers;

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
}

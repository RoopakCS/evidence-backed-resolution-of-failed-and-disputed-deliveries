using System.Security.Claims;

namespace Docket.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(
        this ClaimsPrincipal principal)
    {
        return principal.FindFirst("sub")?.Value
            ?? principal.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;
    }

    public static string? GetRole(
        this ClaimsPrincipal principal)
    {
        return principal.FindFirst("role")?.Value
            ?? principal.FindFirst(
                ClaimTypes.Role)?.Value;
    }
}
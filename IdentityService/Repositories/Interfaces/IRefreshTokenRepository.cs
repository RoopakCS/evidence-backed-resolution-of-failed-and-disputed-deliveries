using IdentityService.Models;

namespace IdentityService.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetActiveByTokenHashAsync(string tokenHash);

    Task AddAsync(RefreshToken refreshToken);

    Task RevokeAsync(RefreshToken refreshToken, Guid? replacedByTokenId = null);

    /// <summary>
    /// Revokes ALL active tokens for a user — used when token-reuse
    /// theft is detected.
    /// </summary>
    Task RevokeAllForUserAsync(Guid userId);
}

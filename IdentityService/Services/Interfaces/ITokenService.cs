using IdentityService.Models;

namespace IdentityService.Services.Interfaces;

/// <summary>
/// Generates and validates JWT access tokens and refresh tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Creates a signed JWT access token for the given user.
    /// Returns the token string and its expiry time (UTC).
    /// </summary>
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);

    /// <summary>
    /// Generates a cryptographically random raw refresh token string
    /// (returned to the client) together with its SHA-256 hash
    /// (stored in the database) and the expiry time (UTC).
    /// </summary>
    (string RawToken, string TokenHash, DateTime ExpiresAt) GenerateRefreshToken();

    /// <summary>
    /// Computes the SHA-256 hash of a raw refresh token for database lookup.
    /// </summary>
    string HashRefreshToken(string rawToken);
}

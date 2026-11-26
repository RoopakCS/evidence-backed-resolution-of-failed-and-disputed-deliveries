using IdentityService.DTOs.Requests;
using IdentityService.DTOs.Responses;
using IdentityService.Exceptions;
using IdentityService.Models;
using IdentityService.Models.Enums;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> RegisterAsync(
        RegisterRequest request,
        string? deviceMetadata = null)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedPhone = request.PhoneNumber.Trim();

        if (await _userRepository.ExistsByEmailAsync(normalizedEmail))
        {
            throw new ServiceException(
                StatusCodes.Status409Conflict,
                "An account with this email already exists.");
        }

        if (await _userRepository.ExistsByPhoneNumberAsync(normalizedPhone))
        {
            throw new ServiceException(
                StatusCodes.Status409Conflict,
                "An account with this phone number already exists.");
        }

        var now = DateTime.UtcNow;

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            PhoneNumber = normalizedPhone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        if (request.Role == Role.Customer)
        {
            user.CustomerProfile = new CustomerProfile
            {
                UserId = user.Id,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        await _userRepository.AddAsync(user);

        return await IssueTokensForUserAsync(user, deviceMetadata);
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        string? deviceMetadata = null)
    {
        var identifier = request.GetNormalizedIdentifier();
        var user = await _userRepository.GetByEmailOrPhoneAsync(identifier);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new ServiceException(
                StatusCodes.Status401Unauthorized,
                "Invalid credentials.");
        }

        if (!user.IsActive)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "This account has been deactivated.");
        }

        return await IssueTokensForUserAsync(user, deviceMetadata);
    }

    public async Task<RefreshResponse> RefreshAsync(
        RefreshRequest request,
        string? deviceMetadata = null)
    {
        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (existingToken is null)
        {
            throw new ServiceException(
                StatusCodes.Status401Unauthorized,
                "Invalid refresh token.");
        }

        if (existingToken.IsRevoked)
        {
            // Token reuse detected — revoke all active tokens for this user
            await _refreshTokenRepository.RevokeAllForUserAsync(existingToken.UserId);

            throw new ServiceException(
                StatusCodes.Status401Unauthorized,
                "Refresh token has been revoked. Please log in again.");
        }

        if (existingToken.IsExpired)
        {
            throw new ServiceException(
                StatusCodes.Status401Unauthorized,
                "Refresh token has expired.");
        }

        var user = existingToken.User;
        if (user is null || !user.IsActive)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "This account is inactive or unavailable.");
        }

        // Rotate refresh token
        var (rawRefreshToken, newTokenHash, refreshExpiresAt) = _tokenService.GenerateRefreshToken();
        var replacementToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newTokenHash,
            ExpiresAt = refreshExpiresAt,
            CreatedAt = DateTime.UtcNow,
            DeviceClientMetadata = deviceMetadata
        };

        await _refreshTokenRepository.AddAsync(replacementToken);
        await _refreshTokenRepository.RevokeAsync(existingToken, replacementToken.Id);

        var (accessToken, accessExpiresAt) = _tokenService.GenerateAccessToken(user);

        return new RefreshResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshToken = rawRefreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt
        };
    }

    public async Task RevokeAsync(RevokeRefreshTokenRequest request)
    {
        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var existingToken = await _refreshTokenRepository.GetActiveByTokenHashAsync(tokenHash);

        if (existingToken is null)
        {
            throw new ServiceException(
                StatusCodes.Status404NotFound,
                "Active refresh token not found.");
        }

        await _refreshTokenRepository.RevokeAsync(existingToken);
    }

    private async Task<LoginResponse> IssueTokensForUserAsync(User user, string? deviceMetadata)
    {
        var (accessToken, accessExpiresAt) = _tokenService.GenerateAccessToken(user);
        var (rawRefreshToken, tokenHash, refreshExpiresAt) = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = refreshExpiresAt,
            CreatedAt = DateTime.UtcNow,
            DeviceClientMetadata = deviceMetadata
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshToken = rawRefreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt,
            User = MapToUserResponse(user)
        };
    }

    private static UserResponse MapToUserResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            IsActive = user.IsActive,
            Address = user.Address
        };
    }
}

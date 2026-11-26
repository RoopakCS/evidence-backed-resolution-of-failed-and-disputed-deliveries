using IdentityService.DTOs.Requests;
using IdentityService.DTOs.Responses;

namespace IdentityService.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request, string? deviceMetadata = null);

    Task<LoginResponse> LoginAsync(LoginRequest request, string? deviceMetadata = null);

    Task<RefreshResponse> RefreshAsync(RefreshRequest request, string? deviceMetadata = null);

    Task RevokeAsync(RevokeRefreshTokenRequest request);
}

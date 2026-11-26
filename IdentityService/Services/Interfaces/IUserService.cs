using IdentityService.DTOs.Requests;
using IdentityService.DTOs.Responses;

namespace IdentityService.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse> GetProfileAsync(Guid userId);

    Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
}

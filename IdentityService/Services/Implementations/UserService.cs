using IdentityService.DTOs.Requests;
using IdentityService.DTOs.Responses;
using IdentityService.Exceptions;
using IdentityService.Models;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new ServiceException(
                StatusCodes.Status404NotFound,
                "User not found.");

        return MapToUserResponse(user);
    }

    public async Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new ServiceException(
                StatusCodes.Status404NotFound,
                "User not found.");

        var normalizedPhone = request.PhoneNumber.Trim();

        if (!string.Equals(user.PhoneNumber, normalizedPhone, StringComparison.Ordinal))
        {
            if (await _userRepository.ExistsByPhoneNumberAsync(normalizedPhone))
            {
                throw new ServiceException(
                    StatusCodes.Status409Conflict,
                    "This phone number is already in use by another account.");
            }
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.PhoneNumber = normalizedPhone;
        user.Address = string.IsNullOrWhiteSpace(request.Address)
            ? null
            : request.Address.Trim();

        await _userRepository.UpdateAsync(user);

        return MapToUserResponse(user);
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

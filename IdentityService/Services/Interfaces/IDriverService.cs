using IdentityService.DTOs.Requests;
using IdentityService.DTOs.Responses;
using IdentityService.Models.Enums;

namespace IdentityService.Services.Interfaces;

public interface IDriverService
{
    Task<DriverProfileResponse> GetDriverProfileAsync(Guid userId);

    Task<DriverProfileResponse> UpsertDriverProfileAsync(Guid userId, DriverProfileRequest request);

    Task<DriverProfileResponse> UpdateAvailabilityAsync(Guid userId, DriverAvailabilityStatus availabilityStatus);

    Task<DriverProfileResponse> UpdateVerificationStatusAsync(Guid driverUserId, DriverVerificationStatus verificationStatus);
}

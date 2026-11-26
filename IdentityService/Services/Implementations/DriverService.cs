using IdentityService.DTOs.Requests;
using IdentityService.DTOs.Responses;
using IdentityService.Exceptions;
using IdentityService.Models;
using IdentityService.Models.Enums;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Services.Implementations;

public class DriverService : IDriverService
{
    private readonly IDriverProfileRepository _driverProfileRepository;
    private readonly IUserRepository _userRepository;

    public DriverService(
        IDriverProfileRepository driverProfileRepository,
        IUserRepository userRepository)
    {
        _driverProfileRepository = driverProfileRepository;
        _userRepository = userRepository;
    }

    public async Task<DriverProfileResponse> GetDriverProfileAsync(Guid userId)
    {
        var profile = await _driverProfileRepository.GetByUserIdAsync(userId)
            ?? throw new ServiceException(
                StatusCodes.Status404NotFound,
                "Driver profile has not been completed yet.");

        return MapToResponse(profile);
    }

    public async Task<DriverProfileResponse> UpsertDriverProfileAsync(Guid userId, DriverProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new ServiceException(
                StatusCodes.Status404NotFound,
                "User not found.");

        if (user.Role != Role.Driver)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "Only users with the Driver role can manage a driver profile.");
        }

        var normalizedLicense = request.LicenseNumber.Trim();

        if (await _driverProfileRepository.ExistsByLicenseNumberAsync(normalizedLicense, userId))
        {
            throw new ServiceException(
                StatusCodes.Status409Conflict,
                "A driver profile with this license number already exists.");
        }

        var existing = await _driverProfileRepository.GetByUserIdAsync(userId);
        var now = DateTime.UtcNow;

        if (existing is null)
        {
            var newProfile = new DriverProfile
            {
                UserId = userId,
                LicenseNumber = normalizedLicense,
                VehicleType = request.VehicleType.Trim(),
                VehicleRegistrationNumber = request.VehicleRegistrationNumber.Trim(),
                VerificationStatus = DriverVerificationStatus.Pending,
                AvailabilityStatus = DriverAvailabilityStatus.Unavailable,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _driverProfileRepository.AddAsync(newProfile);

            return MapToResponse(newProfile);
        }

        existing.LicenseNumber = normalizedLicense;
        existing.VehicleType = request.VehicleType.Trim();
        existing.VehicleRegistrationNumber = request.VehicleRegistrationNumber.Trim();

        // Re-require verification if license or vehicle registration changed
        await _driverProfileRepository.UpdateAsync(existing);

        return MapToResponse(existing);
    }

    public async Task<DriverProfileResponse> UpdateAvailabilityAsync(
        Guid userId,
        DriverAvailabilityStatus availabilityStatus)
    {
        var profile = await _driverProfileRepository.GetByUserIdAsync(userId)
            ?? throw new ServiceException(
                StatusCodes.Status404NotFound,
                "Driver profile has not been completed yet.");

        if (availabilityStatus == DriverAvailabilityStatus.Available &&
            profile.VerificationStatus != DriverVerificationStatus.Verified)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "Driver must be verified before becoming available.");
        }

        profile.AvailabilityStatus = availabilityStatus;
        await _driverProfileRepository.UpdateAsync(profile);

        return MapToResponse(profile);
    }

    public async Task<DriverProfileResponse> UpdateVerificationStatusAsync(
        Guid driverUserId,
        DriverVerificationStatus verificationStatus)
    {
        var profile = await _driverProfileRepository.GetByUserIdAsync(driverUserId)
            ?? throw new ServiceException(
                StatusCodes.Status404NotFound,
                "Driver profile not found.");

        profile.VerificationStatus = verificationStatus;

        if (verificationStatus != DriverVerificationStatus.Verified)
        {
            profile.AvailabilityStatus = DriverAvailabilityStatus.Unavailable;
        }

        await _driverProfileRepository.UpdateAsync(profile);

        return MapToResponse(profile);
    }

    private static DriverProfileResponse MapToResponse(DriverProfile profile)
    {
        return new DriverProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            LicenseNumber = profile.LicenseNumber,
            VehicleType = profile.VehicleType,
            VehicleRegistrationNumber = profile.VehicleRegistrationNumber,
            VerificationStatus = profile.VerificationStatus,
            AvailabilityStatus = profile.AvailabilityStatus,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
    }
}

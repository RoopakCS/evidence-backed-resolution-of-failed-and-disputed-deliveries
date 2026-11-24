using IdentityService.Models.Enums;

namespace IdentityService.DTOs.Responses;

public class DriverProfileResponse
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string LicenseNumber { get; set; } = string.Empty;

    public string VehicleType { get; set; } = string.Empty;

    public string VehicleRegistrationNumber { get; set; } = string.Empty;

    public DriverVerificationStatus VerificationStatus { get; set; }

    public DriverAvailabilityStatus AvailabilityStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

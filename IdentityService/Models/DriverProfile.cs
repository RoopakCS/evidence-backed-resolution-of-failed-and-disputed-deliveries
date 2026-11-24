using IdentityService.Models.Enums;

namespace IdentityService.Models;

public class DriverProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public string LicenseNumber { get; set; } = string.Empty;

    public string VehicleType { get; set; } = string.Empty;

    public string VehicleRegistrationNumber { get; set; } = string.Empty;

    public DriverVerificationStatus VerificationStatus { get; set; } = DriverVerificationStatus.Pending;

    public DriverAvailabilityStatus AvailabilityStatus { get; set; } = DriverAvailabilityStatus.Unavailable;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User? User { get; set; }
}

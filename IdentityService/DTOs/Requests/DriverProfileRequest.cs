using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs.Requests;

public class DriverProfileRequest
{
    [Required(ErrorMessage = "License number is required.")]
    [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters.")]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vehicle type is required.")]
    [StringLength(50, ErrorMessage = "Vehicle type cannot exceed 50 characters.")]
    public string VehicleType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vehicle registration number is required.")]
    [StringLength(50, ErrorMessage = "Vehicle registration number cannot exceed 50 characters.")]
    public string VehicleRegistrationNumber { get; set; } = string.Empty;
}

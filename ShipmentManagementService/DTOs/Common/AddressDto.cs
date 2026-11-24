using System.ComponentModel.DataAnnotations;

namespace ShipmentManagementService.DTOs.Common;

public class AddressDto
{
    [Required(ErrorMessage = "Address line 1 is required.")]
    [StringLength(200, ErrorMessage = "Address line 1 cannot exceed 200 characters.")]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Address line 2 cannot exceed 200 characters.")]
    public string? AddressLine2 { get; set; }

    [Required(ErrorMessage = "City is required.")]
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required.")]
    [StringLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postal code is required.")]
    [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters.")]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required.")]
    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
    public string Country { get; set; } = string.Empty;

    [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90 degrees.")]
    public double? Latitude { get; set; }

    [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180 degrees.")]
    public double? Longitude { get; set; }
}

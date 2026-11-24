using System.ComponentModel.DataAnnotations;

namespace ShipmentManagementService.DTOs.Requests;

public class UpdateShipmentRequest
{
    [StringLength(150, ErrorMessage = "Recipient name cannot exceed 150 characters.")]
    public string? RecipientName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(20, ErrorMessage = "Recipient phone cannot exceed 20 characters.")]
    public string? RecipientPhone { get; set; }

    [StringLength(200, ErrorMessage = "Address line 1 cannot exceed 200 characters.")]
    public string? AddressLine1 { get; set; }

    [StringLength(200, ErrorMessage = "Address line 2 cannot exceed 200 characters.")]
    public string? AddressLine2 { get; set; }

    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
    public string? City { get; set; }

    [StringLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
    public string? State { get; set; }

    [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters.")]
    public string? PostalCode { get; set; }

    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
    public string? Country { get; set; }

    [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90 degrees.")]
    public double? Latitude { get; set; }

    [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180 degrees.")]
    public double? Longitude { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }
}

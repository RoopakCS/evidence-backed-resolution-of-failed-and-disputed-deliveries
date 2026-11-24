using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.DTOs.Responses;

public class ShipmentResponse
{
    public Guid Id { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string RecipientPhone { get; set; } = string.Empty;

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public ShipmentStatus Status { get; set; }

    public DateTime ExpectedDeliveryDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Delivery instructions associated with this shipment. Populated on detail views.
    /// </summary>
    public List<InstructionResponse> Instructions { get; set; } = new();
}

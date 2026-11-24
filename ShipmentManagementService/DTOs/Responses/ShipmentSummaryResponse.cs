using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.DTOs.Responses;

public class ShipmentSummaryResponse
{
    public Guid Id { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public ShipmentStatus Status { get; set; }

    public DateTime ExpectedDeliveryDate { get; set; }

    public DateTime CreatedAt { get; set; }
}

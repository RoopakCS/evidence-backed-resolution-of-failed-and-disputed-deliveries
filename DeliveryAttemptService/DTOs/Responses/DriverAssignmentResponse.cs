using DeliveryAttemptService.DTOs.Common;

namespace DeliveryAttemptService.DTOs.Responses;

/// <summary>
/// Projection of shipment/task assigned to an authorized driver.
/// </summary>
public class DriverAssignmentResponse
{
    public Guid ShipmentId { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;

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

    public string DeliveryState { get; set; } = string.Empty;

    public List<DriverInstructionSummaryDto> Instructions { get; set; } = new();
}

namespace DeliveryAttemptService.Models;

/// <summary>
/// Local projection of a shipment assigned to a driver for delivery.
/// External IDs are plain Guids with no cross-service FK.
/// </summary>
public class DriverAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ShipmentId { get; set; }

    public Guid DriverId { get; set; }

    public Guid CustomerId { get; set; }

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

    public string DeliveryState { get; set; } = "Assigned";

    public DateTime ScheduledDate { get; set; } = DateTime.UtcNow.Date;

    /// <summary>
    /// JSON-serialized list of DriverInstructionSummaryDto snapshots for offline-first access.
    /// </summary>
    public string InstructionsJson { get; set; } = "[]";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

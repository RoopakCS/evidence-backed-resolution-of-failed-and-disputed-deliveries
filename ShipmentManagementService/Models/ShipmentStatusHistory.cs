using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.Models;

/// <summary>
/// Insert-only record of shipment status changes and tracking scans.
/// </summary>
public class ShipmentStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ShipmentId { get; set; }

    public ShipmentStatus Status { get; set; }

    public string? Note { get; set; }

    public string? Location { get; set; }

    public Guid? RecordedByUserId { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public Shipment? Shipment { get; set; }
}

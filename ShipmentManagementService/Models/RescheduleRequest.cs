using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.Models;

public class RescheduleRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ShipmentId { get; set; }

    public DateTime RequestedDeliveryDate { get; set; }

    public string Reason { get; set; } = string.Empty;

    public RescheduleStatus Status { get; set; } = RescheduleStatus.Pending;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedAt { get; set; }

    public Guid? ProcessedByUserId { get; set; }

    // Navigation property
    public Shipment? Shipment { get; set; }
}

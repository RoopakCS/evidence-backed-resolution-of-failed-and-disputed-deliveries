using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.Models;

public class Return
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ShipmentId { get; set; }

    public string ReturnReason { get; set; } = string.Empty;

    public ReturnStatus Status { get; set; } = ReturnStatus.Initiated;

    public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Shipment? Shipment { get; set; }
}

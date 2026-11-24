using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.Models;

public class DeliveryInstruction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ShipmentId { get; set; }

    public InstructionType InstructionType { get; set; }

    public string InstructionText { get; set; } = string.Empty;

    public InstructionStatus Status { get; set; } = InstructionStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Shipment? Shipment { get; set; }
}

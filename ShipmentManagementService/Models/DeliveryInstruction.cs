using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.Models;

public class DeliveryInstruction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ShipmentId { get; set; }

    public InstructionType InstructionType { get; set; } = InstructionType.DeliveryLocation;

    public string DropOffPlace { get; set; } = string.Empty;

    public string? GateOrAccessCode { get; set; }

    public string? Floor { get; set; }

    public string? Door { get; set; }

    public string? InstructionText { get; set; }

    public string? PhotoUrl { get; set; }

    public InstructionStatus Status { get; set; } = InstructionStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Shipment? Shipment { get; set; }
}

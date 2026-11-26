using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.DTOs.Responses;

public class InstructionResponse
{
    public Guid Id { get; set; }

    public Guid ShipmentId { get; set; }

    public InstructionType Type { get; set; }

    public string DropOffPlace { get; set; } = string.Empty;

    public string? GateOrAccessCode { get; set; }

    public string? Floor { get; set; }

    public string? Door { get; set; }

    public string? Text { get; set; }

    public string? PhotoUrl { get; set; }

    public InstructionStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

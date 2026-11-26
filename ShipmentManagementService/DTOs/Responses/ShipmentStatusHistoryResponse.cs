using ShipmentManagementService.Models.Enums;

namespace ShipmentManagementService.DTOs.Responses;

public class ShipmentStatusHistoryResponse
{
    public Guid Id { get; set; }

    public ShipmentStatus Status { get; set; }

    public string? Note { get; set; }

    public string? Location { get; set; }

    public DateTime OccurredAt { get; set; }
}

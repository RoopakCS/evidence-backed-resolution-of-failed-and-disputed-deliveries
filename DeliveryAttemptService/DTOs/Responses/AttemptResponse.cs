using DeliveryAttemptService.Models.Enums;

namespace DeliveryAttemptService.DTOs.Responses;

public class AttemptResponse
{
    public Guid AttemptId { get; set; }

    public Guid ShipmentId { get; set; }

    public Guid DriverId { get; set; }

    public AttemptResult Result { get; set; }

    public FailureReason? FailureReason { get; set; }

    public string? AttemptNote { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public DateTime CapturedAt { get; set; }

    public DateTime ReceivedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<EvidenceResponse> Evidence { get; set; } = new();
}

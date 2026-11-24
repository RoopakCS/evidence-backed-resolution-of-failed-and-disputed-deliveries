namespace DeliveryAttemptService.DTOs.Common;

public class OfflineActionResultDto
{
    public string ActionId { get; set; } = string.Empty;

    public string IdempotencyKey { get; set; } = string.Empty;

    /// <summary>
    /// Status: Accepted | Rejected | AlreadyProcessed
    /// </summary>
    public string Status { get; set; } = string.Empty;

    public Guid? ServerEntityId { get; set; }

    public string? ErrorCode { get; set; }

    public string? Message { get; set; }
}

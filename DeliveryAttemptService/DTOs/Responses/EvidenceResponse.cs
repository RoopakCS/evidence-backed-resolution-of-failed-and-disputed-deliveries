using DeliveryAttemptService.Models.Enums;

namespace DeliveryAttemptService.DTOs.Responses;

public class EvidenceResponse
{
    public Guid EvidenceId { get; set; }

    public Guid AttemptId { get; set; }

    public EvidenceType EvidenceType { get; set; }

    public string MediaType { get; set; } = string.Empty;

    public string ContentHash { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Short-lived presigned download URL provided only when authorized and requested.
    /// </summary>
    public string? DownloadUrl { get; set; }
}

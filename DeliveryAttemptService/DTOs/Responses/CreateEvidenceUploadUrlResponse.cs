namespace DeliveryAttemptService.DTOs.Responses;

public class CreateEvidenceUploadUrlResponse
{
    public Guid EvidenceId { get; set; }

    /// <summary>
    /// Short-lived presigned upload URL.
    /// </summary>
    public string UploadUrl { get; set; } = string.Empty;

    /// <summary>
    /// Object key if safe to expose to client.
    /// </summary>
    public string? ObjectKey { get; set; }

    public DateTime Expiration { get; set; }

    public Dictionary<string, string> RequiredUploadHeaders { get; set; } = new();
}

using DeliveryAttemptService.Models.Enums;

namespace DeliveryAttemptService.Models;

/// <summary>
/// Insert-only evidence record. The physical file is stored in private S3;
/// this database entity holds metadata, object key, and content hash.
/// </summary>
public class Evidence
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Local FK to the delivery attempt this evidence supports.
    /// </summary>
    public Guid AttemptId { get; set; }

    /// <summary>
    /// Object key / path in private S3 storage.
    /// </summary>
    public string ObjectKey { get; set; } = string.Empty;

    /// <summary>
    /// SHA256 checksum/hash of the file content.
    /// </summary>
    public string ContentHash { get; set; } = string.Empty;

    public string MediaType { get; set; } = string.Empty;

    public long? FileSize { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CapturedAt { get; set; }

    public EvidenceType EvidenceType { get; set; }

    public Guid CreatedByDriverId { get; set; }

    // Navigation property
    public DeliveryAttempt? Attempt { get; set; }
}

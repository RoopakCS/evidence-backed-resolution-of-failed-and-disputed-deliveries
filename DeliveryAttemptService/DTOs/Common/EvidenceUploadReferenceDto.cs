using DeliveryAttemptService.Models.Enums;

namespace DeliveryAttemptService.DTOs.Common;

public class EvidenceUploadReferenceDto
{
    public Guid EvidenceId { get; set; }

    public EvidenceType EvidenceType { get; set; }

    public string MediaType { get; set; } = string.Empty;

    public string? ContentHash { get; set; }
}

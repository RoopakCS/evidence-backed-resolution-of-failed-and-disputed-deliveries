using System.ComponentModel.DataAnnotations;

namespace DeliveryAttemptService.DTOs.Requests;

public class ConfirmEvidenceUploadRequest
{
    [Required(ErrorMessage = "EvidenceId is required.")]
    public Guid EvidenceId { get; set; }

    [Required(ErrorMessage = "ContentHash is required.")]
    [StringLength(128, ErrorMessage = "ContentHash cannot exceed 128 characters.")]
    public string ContentHash { get; set; } = string.Empty;

    public Dictionary<string, string>? Metadata { get; set; }

    public bool UploadConfirmation { get; set; } = true;
}

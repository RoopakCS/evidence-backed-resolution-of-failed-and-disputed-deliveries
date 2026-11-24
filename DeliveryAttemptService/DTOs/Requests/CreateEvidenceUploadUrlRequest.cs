using System.ComponentModel.DataAnnotations;
using DeliveryAttemptService.Models.Enums;

namespace DeliveryAttemptService.DTOs.Requests;

public class CreateEvidenceUploadUrlRequest
{
    [Required(ErrorMessage = "AttemptId is required.")]
    public Guid AttemptId { get; set; }

    [Required(ErrorMessage = "FileName is required.")]
    [StringLength(255, ErrorMessage = "FileName cannot exceed 255 characters.")]
    public string FileName { get; set; } = string.Empty;

    [Required(ErrorMessage = "MediaType is required.")]
    [StringLength(100, ErrorMessage = "MediaType cannot exceed 100 characters.")]
    public string MediaType { get; set; } = string.Empty;

    [Range(1, 104857600, ErrorMessage = "FileSize must be between 1 byte and 100MB.")]
    public long? FileSize { get; set; }

    [Required(ErrorMessage = "EvidenceType is required.")]
    public EvidenceType EvidenceType { get; set; }
}

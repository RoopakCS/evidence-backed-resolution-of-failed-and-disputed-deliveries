using System.ComponentModel.DataAnnotations;

namespace DeliveryAttemptService.DTOs.Common;

public class OfflineActionDto
{
    [Required]
    public string ActionId { get; set; } = string.Empty;

    [Required]
    public string ActionType { get; set; } = string.Empty;

    [Required]
    public string IdempotencyKey { get; set; } = string.Empty;

    [Required]
    public DateTime CapturedAt { get; set; }

    /// <summary>
    /// Serialized JSON payload of the action (e.g. attempt record or instruction decision).
    /// </summary>
    [Required]
    public string PayloadJson { get; set; } = string.Empty;
}

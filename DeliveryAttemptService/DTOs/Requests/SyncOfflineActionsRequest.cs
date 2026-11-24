using System.ComponentModel.DataAnnotations;
using DeliveryAttemptService.DTOs.Common;

namespace DeliveryAttemptService.DTOs.Requests;

public class SyncOfflineActionsRequest
{
    [Required(ErrorMessage = "At least one offline action is required.")]
    [MinLength(1, ErrorMessage = "Action batch cannot be empty.")]
    public List<OfflineActionDto> Actions { get; set; } = new();
}

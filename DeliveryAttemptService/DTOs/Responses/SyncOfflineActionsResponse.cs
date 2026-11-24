using DeliveryAttemptService.DTOs.Common;

namespace DeliveryAttemptService.DTOs.Responses;

public class SyncOfflineActionsResponse
{
    public List<OfflineActionResultDto> Results { get; set; } = new();
}

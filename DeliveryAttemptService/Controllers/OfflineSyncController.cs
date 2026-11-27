using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;
using DeliveryAttemptService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAttemptService.Controllers;

[ApiController]
[Route("api/driver/sync")]
[Authorize(Roles = "Driver")]
public class OfflineSyncController : ControllerBase
{
    private readonly IOfflineSyncService _offlineSyncService;

    public OfflineSyncController(IOfflineSyncService offlineSyncService)
    {
        _offlineSyncService = offlineSyncService;
    }

    [HttpPost]
    public async Task<ActionResult<SyncOfflineActionsResponse>> Sync(
        [FromBody] SyncOfflineActionsRequest request,
        CancellationToken ct)
    {
        var driverId = User.GetRequiredUserId();
        var response = await _offlineSyncService.SyncAsync(driverId, request, ct);
        return Ok(response);
    }
}

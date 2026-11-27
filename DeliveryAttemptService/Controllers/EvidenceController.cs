using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;
using DeliveryAttemptService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAttemptService.Controllers;

[ApiController]
[Route("api/evidence")]
[Authorize]
public class EvidenceController : ControllerBase
{
    private readonly IEvidenceService _evidenceService;

    public EvidenceController(IEvidenceService evidenceService)
    {
        _evidenceService = evidenceService;
    }

    [HttpPost("upload-url")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<CreateEvidenceUploadUrlResponse>> CreateUploadUrl(
        [FromBody] CreateEvidenceUploadUrlRequest request,
        CancellationToken ct)
    {
        var driverId = User.GetRequiredUserId();
        var response = await _evidenceService.CreateUploadUrlAsync(driverId, request, ct);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("confirm")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<EvidenceResponse>> ConfirmUpload(
        [FromBody] ConfirmEvidenceUploadRequest request,
        CancellationToken ct)
    {
        var driverId = User.GetRequiredUserId();
        var response = await _evidenceService.ConfirmUploadAsync(driverId, request, ct);
        return Ok(response);
    }

    [HttpGet("attempt/{attemptId:guid}")]
    [Authorize(Roles = "Driver,SupportAgent,Administrator,Customer")]
    public async Task<ActionResult<List<EvidenceResponse>>> GetEvidenceByAttempt(
        Guid attemptId,
        CancellationToken ct)
    {
        var callerId = User.GetRequiredUserId();
        var callerRole = User.GetRequiredRole();
        var items = await _evidenceService.GetEvidenceByAttemptAsync(attemptId, callerId, callerRole, ct);
        return Ok(items);
    }
}

using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;
using DeliveryAttemptService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAttemptService.Controllers;

[ApiController]
[Route("api/attempts")]
[Authorize]
public class AttemptsController : ControllerBase
{
    private readonly IAttemptService _attemptService;

    public AttemptsController(IAttemptService attemptService)
    {
        _attemptService = attemptService;
    }

    [HttpPost]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<AttemptResponse>> RecordAttempt(
        [FromBody] RecordAttemptRequest request,
        CancellationToken ct)
    {
        var driverId = User.GetRequiredUserId();
        var (response, alreadyProcessed) = await _attemptService.RecordAttemptAsync(driverId, request, ct);

        if (alreadyProcessed)
            return Ok(response);

        return CreatedAtAction(nameof(GetAttemptById), new { attemptId = response.AttemptId }, response);
    }

    [HttpGet("{attemptId:guid}")]
    [Authorize(Roles = "Driver,SupportAgent,Administrator,Customer")]
    public async Task<ActionResult<AttemptResponse>> GetAttemptById(
        Guid attemptId,
        CancellationToken ct)
    {
        var callerId = User.GetRequiredUserId();
        var callerRole = User.GetRequiredRole();
        var attempt = await _attemptService.GetAttemptByIdAsync(attemptId, callerId, callerRole, ct);
        return Ok(attempt);
    }

    [HttpGet("shipment/{shipmentId:guid}")]
    [Authorize(Roles = "Driver,SupportAgent,Administrator,Customer")]
    public async Task<ActionResult<List<AttemptResponse>>> GetAttemptsByShipment(
        Guid shipmentId,
        CancellationToken ct)
    {
        var callerId = User.GetRequiredUserId();
        var callerRole = User.GetRequiredRole();
        var attempts = await _attemptService.GetAttemptsByShipmentAsync(shipmentId, callerId, callerRole, ct);
        return Ok(attempts);
    }
}

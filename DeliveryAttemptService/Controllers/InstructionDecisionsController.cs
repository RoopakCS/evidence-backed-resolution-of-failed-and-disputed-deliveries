using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;
using DeliveryAttemptService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAttemptService.Controllers;

[ApiController]
[Route("api/driver/instructions")]
[Authorize]
public class InstructionDecisionsController : ControllerBase
{
    private readonly IInstructionDecisionService _decisionService;

    public InstructionDecisionsController(IInstructionDecisionService decisionService)
    {
        _decisionService = decisionService;
    }

    [HttpPost("decision")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<InstructionDecisionResponse>> RecordDecision(
        [FromBody] InstructionDecisionRequest request,
        CancellationToken ct)
    {
        var driverId = User.GetRequiredUserId();
        var (response, alreadyProcessed) = await _decisionService.RecordDecisionAsync(driverId, request, ct);

        if (alreadyProcessed)
            return Ok(response);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("shipment/{shipmentId:guid}/decisions")]
    [Authorize(Roles = "Driver,SupportAgent,Administrator,Customer")]
    public async Task<ActionResult<List<InstructionDecisionResponse>>> GetDecisionsByShipment(
        Guid shipmentId,
        CancellationToken ct)
    {
        var callerId = User.GetRequiredUserId();
        var callerRole = User.GetRequiredRole();
        var decisions = await _decisionService.GetDecisionsByShipmentAsync(shipmentId, callerId, callerRole, ct);
        return Ok(decisions);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentManagementService.DTOs.Requests;
using ShipmentManagementService.DTOs.Responses;
using ShipmentManagementService.Services.Interfaces;

namespace ShipmentManagementService.Controllers;

[ApiController]
[Route("api/shipments/{shipmentId:guid}/reschedules")]
[Authorize]
public class ReschedulesController : ControllerBase
{
    private readonly IRescheduleService _reschedules;

    public ReschedulesController(IRescheduleService reschedules)
    {
        _reschedules = reschedules;
    }

    [HttpGet]
    public async Task<ActionResult<List<RescheduleResponse>>> List(
        Guid shipmentId,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var result = await _reschedules.ListByShipmentAsync(shipmentId, userId, role, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Customer,SupportAgent,Administrator")]
    public async Task<ActionResult<RescheduleResponse>> Create(
        Guid shipmentId,
        [FromBody] RescheduleRequestDto request,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var key = Request.GetIdempotencyKey();
        var result = await _reschedules.CreateAsync(shipmentId, request, userId, role, key, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("{requestId:guid}/process")]
    [Authorize(Roles = "SupportAgent,Administrator")]
    public async Task<ActionResult<RescheduleResponse>> Process(
        Guid shipmentId,
        Guid requestId,
        [FromBody] ProcessRescheduleRequest request,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var key = Request.GetIdempotencyKey();
        var result = await _reschedules.ProcessAsync(shipmentId, requestId, request, userId, key, ct);
        return Ok(result);
    }
}

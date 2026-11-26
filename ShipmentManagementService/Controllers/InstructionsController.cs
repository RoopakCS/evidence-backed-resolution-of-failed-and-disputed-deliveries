using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentManagementService.DTOs.Requests;
using ShipmentManagementService.DTOs.Responses;
using ShipmentManagementService.Services.Interfaces;

namespace ShipmentManagementService.Controllers;

[ApiController]
[Route("api/shipments/{shipmentId:guid}/instructions")]
[Authorize]
public class InstructionsController : ControllerBase
{
    private readonly IInstructionService _instructions;

    public InstructionsController(IInstructionService instructions)
    {
        _instructions = instructions;
    }

    [HttpGet]
    public async Task<ActionResult<List<InstructionResponse>>> List(
        Guid shipmentId,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var result = await _instructions.ListByShipmentAsync(shipmentId, userId, role, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Customer,SupportAgent,Administrator")]
    public async Task<ActionResult<InstructionResponse>> Create(
        Guid shipmentId,
        [FromBody] CreateInstructionRequest request,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var key = Request.GetIdempotencyKey();
        var result = await _instructions.CreateAsync(shipmentId, request, userId, role, key, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{instructionId:guid}")]
    [Authorize(Roles = "Customer,SupportAgent,Administrator")]
    public async Task<ActionResult<InstructionResponse>> Update(
        Guid shipmentId,
        Guid instructionId,
        [FromBody] UpdateInstructionRequest request,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var key = Request.GetIdempotencyKey();
        var result = await _instructions.UpdateAsync(shipmentId, instructionId, request, userId, role, key, ct);
        return Ok(result);
    }
}

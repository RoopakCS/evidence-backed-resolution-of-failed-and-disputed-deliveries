using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentManagementService.DTOs.Requests;
using ShipmentManagementService.DTOs.Responses;
using ShipmentManagementService.Models.Enums;
using ShipmentManagementService.Services.Interfaces;

namespace ShipmentManagementService.Controllers;

public sealed class UpdateReturnStatusRequest
{
    [Required]
    public ReturnStatus Status { get; set; }
}

[ApiController]
[Route("api/shipments/{shipmentId:guid}/returns")]
[Authorize]
public class ReturnsController : ControllerBase
{
    private readonly IReturnService _returns;

    public ReturnsController(IReturnService returns)
    {
        _returns = returns;
    }

    [HttpGet]
    public async Task<ActionResult<List<ReturnResponse>>> List(
        Guid shipmentId,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var result = await _returns.ListByShipmentAsync(shipmentId, userId, role, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Customer,SupportAgent,Administrator")]
    public async Task<ActionResult<ReturnResponse>> Initiate(
        Guid shipmentId,
        [FromBody] InitiateReturnRequest request,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var key = Request.GetIdempotencyKey();
        var result = await _returns.InitiateAsync(shipmentId, request, userId, role, key, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPatch("{returnId:guid}/status")]
    [Authorize(Roles = "SupportAgent,Administrator")]
    public async Task<ActionResult<ReturnResponse>> UpdateStatus(
        Guid shipmentId,
        Guid returnId,
        [FromBody] UpdateReturnStatusRequest request,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var result = await _returns.UpdateStatusAsync(shipmentId, returnId, request.Status, userId, ct);
        return Ok(result);
    }
}

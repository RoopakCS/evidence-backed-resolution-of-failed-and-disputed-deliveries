using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentManagementService.DTOs.Requests;
using ShipmentManagementService.DTOs.Responses;
using ShipmentManagementService.Services.Interfaces;

namespace ShipmentManagementService.Controllers;

[ApiController]
[Route("api/shipments")]
[Authorize]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipments;

    public ShipmentsController(IShipmentService shipments)
    {
        _shipments = shipments;
    }

    /// <summary>
    /// Public tracking lookup by tracking number (returns non-PII status and age).
    /// </summary>
    [HttpGet("track/{trackingNumber}")]
    [AllowAnonymous]
    public async Task<ActionResult<PublicTrackingResponse>> Track(
        string trackingNumber,
        CancellationToken ct)
    {
        var result = await _shipments.TrackByNumberAsync(trackingNumber, ct);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ShipmentSummaryResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var result = await _shipments.ListAsync(userId, role, page, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShipmentResponse>> GetById(Guid id, CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var result = await _shipments.GetByIdAsync(id, userId, role, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Customer,SupportAgent,Administrator")]
    public async Task<ActionResult<ShipmentResponse>> Create(
        [FromBody] CreateShipmentRequest request,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var result = await _shipments.CreateAsync(request, userId, role, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Customer,SupportAgent,Administrator")]
    public async Task<ActionResult<ShipmentResponse>> Update(
        Guid id,
        [FromBody] UpdateShipmentRequest request,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var result = await _shipments.UpdateAsync(id, request, userId, role, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<List<ShipmentStatusHistoryResponse>>> GetHistory(
        Guid id,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var role = User.GetRequiredRole();
        var result = await _shipments.GetStatusHistoryAsync(id, userId, role, ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/tracking")]
    [Authorize(Roles = "SupportAgent,Administrator")]
    public async Task<ActionResult<ShipmentStatusHistoryResponse>> RecordTrackingUpdate(
        Guid id,
        [FromBody] RecordTrackingUpdateRequest request,
        CancellationToken ct)
    {
        var userId = User.GetRequiredUserId();
        var result = await _shipments.RecordTrackingUpdateAsync(id, request, userId, ct);
        return Ok(result);
    }
}

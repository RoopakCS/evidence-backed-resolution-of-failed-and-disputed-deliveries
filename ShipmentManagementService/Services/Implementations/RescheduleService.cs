using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ShipmentManagementService.Data;
using ShipmentManagementService.DTOs.Requests;
using ShipmentManagementService.DTOs.Responses;
using ShipmentManagementService.Exceptions;
using ShipmentManagementService.Models;
using ShipmentManagementService.Models.Enums;
using ShipmentManagementService.Services.Interfaces;
using ShipmentManagementService.Services.Mapping;

namespace ShipmentManagementService.Services.Implementations;

public class RescheduleService : IRescheduleService
{
    private readonly ShipmentDbContext _db;
    private readonly IIdempotencyService _idempotency;

    public RescheduleService(ShipmentDbContext db, IIdempotencyService idempotency)
    {
        _db = db;
        _idempotency = idempotency;
    }

    public async Task<List<RescheduleResponse>> ListByShipmentAsync(
        Guid shipmentId,
        Guid actorUserId,
        string actorRole,
        CancellationToken ct = default)
    {
        var shipment = await _db.Shipments
            .AsNoTracking()
            .Include(s => s.RescheduleRequests)
            .FirstOrDefaultAsync(s => s.Id == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        ShipmentService.EnsureReadAccess(shipment, actorUserId, actorRole);

        return shipment.RescheduleRequests
            .OrderByDescending(r => r.RequestedAt)
            .Select(ShipmentMapper.ToResponse)
            .ToList();
    }

    public async Task<RescheduleResponse> CreateAsync(
        Guid shipmentId,
        RescheduleRequestDto request,
        Guid actorUserId,
        string actorRole,
        string? idempotencyKey = null,
        CancellationToken ct = default)
    {
        var endpoint = $"POST:/api/shipments/{shipmentId}/reschedules";
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var existing = await _idempotency.GetAsync(idempotencyKey, actorUserId, endpoint, ct);
            if (existing is not null)
                return JsonSerializer.Deserialize<RescheduleResponse>(existing.ResponseBody)!;
        }

        var shipment = await _db.Shipments.FirstOrDefaultAsync(s => s.Id == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        ShipmentService.EnsureWriteAccess(shipment, actorUserId, actorRole);

        if (ShipmentService.IsTerminalOrLocked(shipment.Status))
            throw new ServiceException(StatusCodes.Status409Conflict,
                $"Cannot reschedule shipment in status '{shipment.Status}'.");

        var requestedUtc = DateTime.SpecifyKind(request.RequestedDeliveryDate, DateTimeKind.Utc);
        if (requestedUtc <= DateTime.UtcNow)
            throw new ServiceException(StatusCodes.Status400BadRequest,
                "Requested delivery date must be in the future.");

        var reschedule = new RescheduleRequest
        {
            ShipmentId = shipmentId,
            RequestedDeliveryDate = requestedUtc,
            Reason = request.Reason.Trim(),
            Status = RescheduleStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        await _db.RescheduleRequests.AddAsync(reschedule, ct);

        var response = ShipmentMapper.ToResponse(reschedule);
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
            _idempotency.StageRecord(idempotencyKey, actorUserId, endpoint, StatusCodes.Status201Created, response);

        await _db.SaveChangesAsync(ct);
        return response;
    }

    public async Task<RescheduleResponse> ProcessAsync(
        Guid shipmentId,
        Guid requestId,
        ProcessRescheduleRequest request,
        Guid actorUserId,
        string? idempotencyKey = null,
        CancellationToken ct = default)
    {
        var endpoint = $"POST:/api/shipments/{shipmentId}/reschedules/{requestId}/process";
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var existing = await _idempotency.GetAsync(idempotencyKey, actorUserId, endpoint, ct);
            if (existing is not null)
                return JsonSerializer.Deserialize<RescheduleResponse>(existing.ResponseBody)!;
        }

        var shipment = await _db.Shipments.FirstOrDefaultAsync(s => s.Id == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        var reschedule = await _db.RescheduleRequests
            .FirstOrDefaultAsync(r => r.Id == requestId && r.ShipmentId == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Reschedule request not found.");

        if (reschedule.Status != RescheduleStatus.Pending)
            throw new ServiceException(StatusCodes.Status409Conflict,
                $"Reschedule request is already '{reschedule.Status}'.");

        var now = DateTime.UtcNow;
        reschedule.ProcessedAt = now;
        reschedule.ProcessedByUserId = actorUserId;

        if (request.IsApproved)
        {
            if (ShipmentService.IsTerminalOrLocked(shipment.Status))
                throw new ServiceException(StatusCodes.Status409Conflict,
                    $"Cannot approve reschedule when shipment is '{shipment.Status}'.");

            reschedule.Status = RescheduleStatus.Approved;
            shipment.ExpectedDeliveryDate = reschedule.RequestedDeliveryDate;
            shipment.Status = ShipmentStatus.Rescheduled;
            shipment.UpdatedAt = now;

            _db.ShipmentStatusHistories.Add(new ShipmentStatusHistory
            {
                ShipmentId = shipment.Id,
                Status = ShipmentStatus.Rescheduled,
                Note = $"Rescheduled to {reschedule.RequestedDeliveryDate:yyyy-MM-dd}.",
                RecordedByUserId = actorUserId,
                OccurredAt = now
            });
        }
        else
        {
            reschedule.Status = RescheduleStatus.Rejected;
            if (!string.IsNullOrWhiteSpace(request.Reason))
                reschedule.Reason = $"{reschedule.Reason} | Decision note: {request.Reason.Trim()}";
        }

        var response = ShipmentMapper.ToResponse(reschedule);
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
            _idempotency.StageRecord(idempotencyKey, actorUserId, endpoint, StatusCodes.Status200OK, response);

        await _db.SaveChangesAsync(ct);
        return response;
    }
}

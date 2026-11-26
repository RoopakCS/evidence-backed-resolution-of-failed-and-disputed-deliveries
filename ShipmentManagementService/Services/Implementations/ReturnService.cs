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

public class ReturnService : IReturnService
{
    private readonly ShipmentDbContext _db;
    private readonly IIdempotencyService _idempotency;

    public ReturnService(ShipmentDbContext db, IIdempotencyService idempotency)
    {
        _db = db;
        _idempotency = idempotency;
    }

    public async Task<List<ReturnResponse>> ListByShipmentAsync(
        Guid shipmentId,
        Guid actorUserId,
        string actorRole,
        CancellationToken ct = default)
    {
        var shipment = await _db.Shipments
            .AsNoTracking()
            .Include(s => s.Returns)
            .FirstOrDefaultAsync(s => s.Id == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        ShipmentService.EnsureReadAccess(shipment, actorUserId, actorRole);

        return shipment.Returns
            .OrderByDescending(r => r.InitiatedAt)
            .Select(ShipmentMapper.ToResponse)
            .ToList();
    }

    public async Task<ReturnResponse> InitiateAsync(
        Guid shipmentId,
        InitiateReturnRequest request,
        Guid actorUserId,
        string actorRole,
        string? idempotencyKey = null,
        CancellationToken ct = default)
    {
        var endpoint = $"POST:/api/shipments/{shipmentId}/returns";
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var existing = await _idempotency.GetAsync(idempotencyKey, actorUserId, endpoint, ct);
            if (existing is not null)
                return JsonSerializer.Deserialize<ReturnResponse>(existing.ResponseBody)!;
        }

        var shipment = await _db.Shipments
            .Include(s => s.Returns)
            .FirstOrDefaultAsync(s => s.Id == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        ShipmentService.EnsureWriteAccess(shipment, actorUserId, actorRole);

        if (shipment.Status is ShipmentStatus.ReturnInitiated or ShipmentStatus.Returned or ShipmentStatus.Cancelled)
            throw new ServiceException(StatusCodes.Status409Conflict,
                $"Cannot initiate return when shipment is '{shipment.Status}'.");

        var hasActiveReturn = shipment.Returns.Any(r =>
            r.Status is ReturnStatus.Initiated or ReturnStatus.Approved or ReturnStatus.InTransit);

        if (hasActiveReturn)
            throw new ServiceException(StatusCodes.Status409Conflict,
                "An active return already exists for this shipment.");

        var now = DateTime.UtcNow;
        var returnEntity = new Return
        {
            ShipmentId = shipmentId,
            ReturnReason = request.ReturnReason.Trim(),
            Status = ReturnStatus.Initiated,
            InitiatedAt = now,
            UpdatedAt = now
        };

        shipment.Status = ShipmentStatus.ReturnInitiated;
        shipment.UpdatedAt = now;

        await _db.Returns.AddAsync(returnEntity, ct);
        _db.ShipmentStatusHistories.Add(new ShipmentStatusHistory
        {
            ShipmentId = shipment.Id,
            Status = ShipmentStatus.ReturnInitiated,
            Note = $"Return initiated: {returnEntity.ReturnReason}",
            RecordedByUserId = actorUserId,
            OccurredAt = now
        });

        var response = ShipmentMapper.ToResponse(returnEntity);
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
            _idempotency.StageRecord(idempotencyKey, actorUserId, endpoint, StatusCodes.Status201Created, response);

        await _db.SaveChangesAsync(ct);
        return response;
    }

    public async Task<ReturnResponse> UpdateStatusAsync(
        Guid shipmentId,
        Guid returnId,
        ReturnStatus newStatus,
        Guid actorUserId,
        CancellationToken ct = default)
    {
        var shipment = await _db.Shipments.FirstOrDefaultAsync(s => s.Id == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        var returnEntity = await _db.Returns
            .FirstOrDefaultAsync(r => r.Id == returnId && r.ShipmentId == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Return not found.");

        var now = DateTime.UtcNow;
        returnEntity.Status = newStatus;
        returnEntity.UpdatedAt = now;

        if (newStatus == ReturnStatus.ReturnedToSender)
        {
            returnEntity.CompletedAt = now;
            shipment.Status = ShipmentStatus.Returned;
            shipment.UpdatedAt = now;

            _db.ShipmentStatusHistories.Add(new ShipmentStatusHistory
            {
                ShipmentId = shipment.Id,
                Status = ShipmentStatus.Returned,
                Note = "Shipment returned to sender.",
                RecordedByUserId = actorUserId,
                OccurredAt = now
            });
        }

        await _db.SaveChangesAsync(ct);
        return ShipmentMapper.ToResponse(returnEntity);
    }
}

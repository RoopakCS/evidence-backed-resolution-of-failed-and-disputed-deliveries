using System.Text.Json;
using Contracts;
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

public class InstructionService : IInstructionService
{
    private readonly ShipmentDbContext _db;
    private readonly IIdempotencyService _idempotency;

    public InstructionService(ShipmentDbContext db, IIdempotencyService idempotency)
    {
        _db = db;
        _idempotency = idempotency;
    }

    public async Task<List<InstructionResponse>> ListByShipmentAsync(
        Guid shipmentId,
        Guid actorUserId,
        string actorRole,
        CancellationToken ct = default)
    {
        var shipment = await _db.Shipments
            .AsNoTracking()
            .Include(s => s.Instructions)
            .FirstOrDefaultAsync(s => s.Id == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        ShipmentService.EnsureReadAccess(shipment, actorUserId, actorRole);

        return shipment.Instructions
            .OrderByDescending(i => i.CreatedAt)
            .Select(ShipmentMapper.ToResponse)
            .ToList();
    }

    public async Task<InstructionResponse> CreateAsync(
        Guid shipmentId,
        CreateInstructionRequest request,
        Guid actorUserId,
        string actorRole,
        string? idempotencyKey = null,
        CancellationToken ct = default)
    {
        var endpoint = $"POST:/api/shipments/{shipmentId}/instructions";
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var existing = await _idempotency.GetAsync(idempotencyKey, actorUserId, endpoint, ct);
            if (existing is not null)
                return JsonSerializer.Deserialize<InstructionResponse>(existing.ResponseBody)!;
        }

        var shipment = await _db.Shipments.FirstOrDefaultAsync(s => s.Id == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        ShipmentService.EnsureWriteAccess(shipment, actorUserId, actorRole);

        if (ShipmentService.IsTerminalOrLocked(shipment.Status))
            throw new ServiceException(StatusCodes.Status409Conflict,
                $"Cannot add delivery instructions when shipment is '{shipment.Status}'.");

        var now = DateTime.UtcNow;
        var instruction = new DeliveryInstruction
        {
            ShipmentId = shipmentId,
            InstructionType = request.InstructionType,
            DropOffPlace = request.DropOffPlace.Trim(),
            GateOrAccessCode = request.GateOrAccessCode?.Trim(),
            Floor = request.Floor?.Trim(),
            Door = request.Door?.Trim(),
            InstructionText = request.InstructionText?.Trim(),
            PhotoUrl = request.PhotoUrl?.Trim(),
            Status = InstructionStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _db.DeliveryInstructions.AddAsync(instruction, ct);
        StageOutboxEvent(instruction, now);

        var response = ShipmentMapper.ToResponse(instruction);
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
            _idempotency.StageRecord(idempotencyKey, actorUserId, endpoint, StatusCodes.Status201Created, response);

        await _db.SaveChangesAsync(ct);
        return response;
    }

    public async Task<InstructionResponse> UpdateAsync(
        Guid shipmentId,
        Guid instructionId,
        UpdateInstructionRequest request,
        Guid actorUserId,
        string actorRole,
        string? idempotencyKey = null,
        CancellationToken ct = default)
    {
        var endpoint = $"PUT:/api/shipments/{shipmentId}/instructions/{instructionId}";
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var existing = await _idempotency.GetAsync(idempotencyKey, actorUserId, endpoint, ct);
            if (existing is not null)
                return JsonSerializer.Deserialize<InstructionResponse>(existing.ResponseBody)!;
        }

        var shipment = await _db.Shipments.FirstOrDefaultAsync(s => s.Id == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Shipment not found.");

        ShipmentService.EnsureWriteAccess(shipment, actorUserId, actorRole);

        if (ShipmentService.IsTerminalOrLocked(shipment.Status))
            throw new ServiceException(StatusCodes.Status409Conflict,
                $"Cannot update delivery instructions when shipment is '{shipment.Status}'.");

        var instruction = await _db.DeliveryInstructions
            .FirstOrDefaultAsync(i => i.Id == instructionId && i.ShipmentId == shipmentId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Delivery instruction not found.");

        if (instruction.Status == InstructionStatus.Completed)
            throw new ServiceException(StatusCodes.Status409Conflict, "Completed instruction cannot be modified.");

        if (request.InstructionType.HasValue) instruction.InstructionType = request.InstructionType.Value;
        if (request.DropOffPlace is not null) instruction.DropOffPlace = request.DropOffPlace.Trim();
        if (request.GateOrAccessCode is not null) instruction.GateOrAccessCode = request.GateOrAccessCode.Trim();
        if (request.Floor is not null) instruction.Floor = request.Floor.Trim();
        if (request.Door is not null) instruction.Door = request.Door.Trim();
        if (request.InstructionText is not null) instruction.InstructionText = request.InstructionText.Trim();
        if (request.PhotoUrl is not null) instruction.PhotoUrl = request.PhotoUrl.Trim();

        var now = DateTime.UtcNow;
        instruction.Status = InstructionStatus.Pending;
        instruction.UpdatedAt = now;

        StageOutboxEvent(instruction, now);

        var response = ShipmentMapper.ToResponse(instruction);
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
            _idempotency.StageRecord(idempotencyKey, actorUserId, endpoint, StatusCodes.Status200OK, response);

        await _db.SaveChangesAsync(ct);
        return response;
    }

    private void StageOutboxEvent(DeliveryInstruction instruction, DateTime occurredAt)
    {
        var evt = new ShipmentInstructionUpdated
        {
            EventId = Guid.NewGuid(),
            OccurredAt = occurredAt,
            ShipmentId = instruction.ShipmentId,
            InstructionId = instruction.Id,
            State = instruction.Status.ToString()
        };

        _db.OutboxMessages.Add(new OutboxMessage
        {
            EventType = nameof(ShipmentInstructionUpdated),
            Payload = JsonSerializer.Serialize(evt),
            OccurredAt = occurredAt
        });
    }
}

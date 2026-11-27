using System.Text.Json;
using Contracts;
using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;
using DeliveryAttemptService.Exceptions;
using DeliveryAttemptService.Models;
using DeliveryAttemptService.Models.Enums;
using DeliveryAttemptService.Repositories.Interfaces;
using DeliveryAttemptService.Services.Interfaces;
using DeliveryAttemptService.Services.Mapping;

namespace DeliveryAttemptService.Services.Implementations;

public class AttemptService : IAttemptService
{
    private const string ActionType = "RecordAttempt";
    private const string Endpoint = "POST /api/attempts";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDeliveryAttemptRepository _attempts;
    private readonly IAssignmentRepository _assignments;
    private readonly IIdempotencyService _idempotency;

    public AttemptService(
        IDeliveryAttemptRepository attempts,
        IAssignmentRepository assignments,
        IIdempotencyService idempotency)
    {
        _attempts = attempts;
        _assignments = assignments;
        _idempotency = idempotency;
    }

    public async Task<(AttemptResponse Response, bool AlreadyProcessed)> RecordAttemptAsync(
        Guid driverId,
        RecordAttemptRequest request,
        CancellationToken ct = default)
    {
        var requestHash = _idempotency.ComputeHash(request);
        var existingRecord = await _idempotency.GetAsync(request.IdempotencyKey.Trim(), driverId, ActionType, ct);
        if (existingRecord is not null)
        {
            if (!string.Equals(existingRecord.RequestHash, requestHash, StringComparison.Ordinal))
            {
                throw new ServiceException(
                    StatusCodes.Status409Conflict,
                    "IdempotencyKey has already been used with a different request payload.");
            }

            var cached = JsonSerializer.Deserialize<AttemptResponse>(existingRecord.ResponseBody, JsonOptions)
                ?? throw new InvalidOperationException("The stored idempotency response is invalid.");
            return (cached, true);
        }

        if (request.Result == AttemptResult.Failed && !request.FailureReason.HasValue)
        {
            throw new ServiceException(
                StatusCodes.Status400BadRequest,
                "FailureReason is required when AttemptResult is Failed.");
        }

        var assignment = await _assignments.GetByShipmentAndDriverAsync(request.ShipmentId, driverId, ct);
        if (assignment is null)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "Shipment is not assigned to the authenticated driver.");
        }

        var now = DateTime.UtcNow;
        var capturedAtUtc = request.CapturedAt.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.CapturedAt, DateTimeKind.Utc)
            : request.CapturedAt.ToUniversalTime();

        var attempt = new DeliveryAttempt
        {
            ShipmentId = request.ShipmentId,
            DriverId = driverId,
            Result = request.Result,
            FailureReason = request.Result == AttemptResult.Failed ? request.FailureReason : null,
            AttemptNote = request.AttemptNote?.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CapturedAt = capturedAtUtc,
            ReceivedAt = now,
            IdempotencyKey = request.IdempotencyKey.Trim(),
            CreatedAt = now
        };

        await _attempts.AddAttemptAsync(attempt, ct);

        // Update local assignment projection state
        assignment.DeliveryState = request.Result == AttemptResult.Delivered ? "Delivered" : "AttemptFailed";
        assignment.UpdatedAt = now;

        // Stage domain event in transactional outbox (for future RabbitMQ dispatch)
        if (request.Result == AttemptResult.Delivered)
        {
            var evt = new DeliveryAttemptCompleted
            {
                EventId = Guid.NewGuid(),
                OccurredAt = now,
                AttemptId = attempt.Id,
                ShipmentId = attempt.ShipmentId
            };
            await _attempts.AddOutboxMessageAsync(new OutboxMessage
            {
                EventType = nameof(DeliveryAttemptCompleted),
                Payload = JsonSerializer.Serialize(evt, JsonOptions),
                OccurredAt = now
            }, ct);
        }
        else
        {
            var evt = new DeliveryAttemptFailed
            {
                EventId = Guid.NewGuid(),
                OccurredAt = now,
                AttemptId = attempt.Id,
                ShipmentId = attempt.ShipmentId,
                Reason = attempt.FailureReason?.ToString() ?? "Other"
            };
            await _attempts.AddOutboxMessageAsync(new OutboxMessage
            {
                EventType = nameof(DeliveryAttemptFailed),
                Payload = JsonSerializer.Serialize(evt, JsonOptions),
                OccurredAt = now
            }, ct);
        }

        var response = AttemptMapper.ToResponse(attempt);
        _idempotency.StageRecord(
            attempt.IdempotencyKey,
            driverId,
            Endpoint,
            ActionType,
            attempt.Id,
            StatusCodes.Status201Created,
            response,
            requestHash);

        await _attempts.SaveChangesAsync(ct);

        return (response, false);
    }

    public async Task<AttemptResponse> GetAttemptByIdAsync(
        Guid attemptId,
        Guid callerId,
        string callerRole,
        CancellationToken ct = default)
    {
        var attempt = await _attempts.GetAttemptByIdAsync(attemptId, includeEvidence: true, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Delivery attempt not found.");

        if (string.Equals(callerRole, "Driver", StringComparison.OrdinalIgnoreCase) && attempt.DriverId != callerId)
        {
            throw new ServiceException(StatusCodes.Status403Forbidden, "You can only view your own delivery attempts.");
        }

        if (string.Equals(callerRole, "Customer", StringComparison.OrdinalIgnoreCase))
            await EnsureCustomerOwnsShipmentAsync(attempt.ShipmentId, callerId, ct);

        return AttemptMapper.ToResponse(attempt);
    }

    public async Task<List<AttemptResponse>> GetAttemptsByShipmentAsync(
        Guid shipmentId,
        Guid callerId,
        string callerRole,
        CancellationToken ct = default)
    {
        if (string.Equals(callerRole, "Driver", StringComparison.OrdinalIgnoreCase))
        {
            var assignment = await _assignments.GetByShipmentAndDriverAsync(shipmentId, callerId, ct);
            if (assignment is null)
            {
                throw new ServiceException(StatusCodes.Status403Forbidden, "You are not assigned to this shipment.");
            }
        }
        else if (string.Equals(callerRole, "Customer", StringComparison.OrdinalIgnoreCase))
        {
            await EnsureCustomerOwnsShipmentAsync(shipmentId, callerId, ct);
        }

        var attempts = await _attempts.GetAttemptsByShipmentAsync(shipmentId, ct);
        return attempts.Select(a => AttemptMapper.ToResponse(a)).ToList();
    }

    private async Task EnsureCustomerOwnsShipmentAsync(
        Guid shipmentId,
        Guid customerId,
        CancellationToken ct)
    {
        if (await _assignments.GetByShipmentAndCustomerAsync(shipmentId, customerId, ct) is null)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "You can only view delivery attempts for your own shipments.");
        }
    }
}

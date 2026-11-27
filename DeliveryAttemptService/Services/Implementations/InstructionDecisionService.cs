using System.Text.Json;
using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;
using DeliveryAttemptService.Exceptions;
using DeliveryAttemptService.Models;
using DeliveryAttemptService.Models.Enums;
using DeliveryAttemptService.Repositories.Interfaces;
using DeliveryAttemptService.Services.Interfaces;
using DeliveryAttemptService.Services.Mapping;

namespace DeliveryAttemptService.Services.Implementations;

public class InstructionDecisionService : IInstructionDecisionService
{
    private const string ActionType = "InstructionDecision";
    private const string Endpoint = "POST /api/driver/instructions/decision";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDeliveryAttemptRepository _attempts;
    private readonly IAssignmentRepository _assignments;
    private readonly IIdempotencyService _idempotency;

    public InstructionDecisionService(
        IDeliveryAttemptRepository attempts,
        IAssignmentRepository assignments,
        IIdempotencyService idempotency)
    {
        _attempts = attempts;
        _assignments = assignments;
        _idempotency = idempotency;
    }

    public async Task<(InstructionDecisionResponse Response, bool AlreadyProcessed)> RecordDecisionAsync(
        Guid driverId,
        InstructionDecisionRequest request,
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

            var cached = JsonSerializer.Deserialize<InstructionDecisionResponse>(existingRecord.ResponseBody, JsonOptions)
                ?? throw new InvalidOperationException("The stored idempotency response is invalid.");
            return (cached, true);
        }

        if (request.Decision == DriverInstructionDecision.Declined && string.IsNullOrWhiteSpace(request.Reason))
        {
            throw new ServiceException(
                StatusCodes.Status400BadRequest,
                "A reason is required when declining a delivery instruction.");
        }

        var assignment = await _assignments.GetByShipmentAndDriverAsync(request.ShipmentId, driverId, ct);
        if (assignment is null)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "Shipment is not assigned to the authenticated driver.");
        }

        var decidedAtUtc = request.DecidedAt.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.DecidedAt, DateTimeKind.Utc)
            : request.DecidedAt.ToUniversalTime();

        var decision = new InstructionDecision
        {
            InstructionId = request.InstructionId,
            ShipmentId = request.ShipmentId,
            DriverId = driverId,
            Decision = request.Decision,
            Reason = request.Reason?.Trim(),
            DecidedAt = decidedAtUtc,
            IdempotencyKey = request.IdempotencyKey.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _attempts.AddInstructionDecisionAsync(decision, ct);

        var response = AttemptMapper.ToResponse(decision);
        _idempotency.StageRecord(
            decision.IdempotencyKey,
            driverId,
            Endpoint,
            ActionType,
            decision.Id,
            StatusCodes.Status201Created,
            response,
            requestHash);

        await _attempts.SaveChangesAsync(ct);

        return (response, false);
    }

    public async Task<List<InstructionDecisionResponse>> GetDecisionsByShipmentAsync(
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
                throw new ServiceException(
                    StatusCodes.Status403Forbidden,
                    "You are not assigned to this shipment.");
            }
        }
        else if (string.Equals(callerRole, "Customer", StringComparison.OrdinalIgnoreCase) &&
                 await _assignments.GetByShipmentAndCustomerAsync(shipmentId, callerId, ct) is null)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "You can only view instruction decisions for your own shipments.");
        }

        var decisions = await _attempts.GetDecisionsByShipmentAsync(shipmentId, ct);
        return decisions.Select(AttemptMapper.ToResponse).ToList();
    }
}

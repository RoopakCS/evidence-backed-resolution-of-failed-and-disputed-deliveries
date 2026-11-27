using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using DeliveryAttemptService.DTOs.Common;
using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;
using DeliveryAttemptService.Exceptions;
using DeliveryAttemptService.Services.Interfaces;

namespace DeliveryAttemptService.Services.Implementations;

public class OfflineSyncService : IOfflineSyncService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IAttemptService _attemptService;
    private readonly IInstructionDecisionService _decisionService;

    public OfflineSyncService(
        IAttemptService attemptService,
        IInstructionDecisionService decisionService)
    {
        _attemptService = attemptService;
        _decisionService = decisionService;
    }

    public async Task<SyncOfflineActionsResponse> SyncAsync(
        Guid driverId,
        SyncOfflineActionsRequest request,
        CancellationToken ct = default)
    {
        var results = new List<OfflineActionResultDto>(request.Actions.Count);

        foreach (var action in request.Actions)
        {
            results.Add(await ProcessActionAsync(driverId, action, ct));
        }

        return new SyncOfflineActionsResponse { Results = results };
    }

    private async Task<OfflineActionResultDto> ProcessActionAsync(
        Guid driverId,
        OfflineActionDto action,
        CancellationToken ct)
    {
        try
        {
            switch (action.ActionType.Trim().ToUpperInvariant())
            {
                case "RECORDATTEMPT":
                case "ATTEMPT":
                {
                    var payload = JsonSerializer.Deserialize<RecordAttemptRequest>(action.PayloadJson, JsonOptions)
                        ?? throw new ServiceException(StatusCodes.Status400BadRequest, "Invalid RecordAttempt payload.");

                    if (string.IsNullOrWhiteSpace(payload.IdempotencyKey))
                        payload.IdempotencyKey = action.IdempotencyKey;

                    if (payload.CapturedAt == default)
                        payload.CapturedAt = action.CapturedAt;

                    ValidatePayload(payload);

                    var (response, alreadyProcessed) = await _attemptService.RecordAttemptAsync(driverId, payload, ct);
                    return new OfflineActionResultDto
                    {
                        ActionId = action.ActionId,
                        IdempotencyKey = action.IdempotencyKey,
                        Status = alreadyProcessed ? "AlreadyProcessed" : "Accepted",
                        ServerEntityId = response.AttemptId,
                        Message = alreadyProcessed ? "Attempt was already processed." : "Attempt recorded."
                    };
                }

                case "INSTRUCTIONDECISION":
                case "DECISION":
                {
                    var payload = JsonSerializer.Deserialize<InstructionDecisionRequest>(action.PayloadJson, JsonOptions)
                        ?? throw new ServiceException(StatusCodes.Status400BadRequest, "Invalid InstructionDecision payload.");

                    if (string.IsNullOrWhiteSpace(payload.IdempotencyKey))
                        payload.IdempotencyKey = action.IdempotencyKey;

                    if (payload.DecidedAt == default)
                        payload.DecidedAt = action.CapturedAt;

                    ValidatePayload(payload);

                    var (response, alreadyProcessed) = await _decisionService.RecordDecisionAsync(driverId, payload, ct);
                    return new OfflineActionResultDto
                    {
                        ActionId = action.ActionId,
                        IdempotencyKey = action.IdempotencyKey,
                        Status = alreadyProcessed ? "AlreadyProcessed" : "Accepted",
                        ServerEntityId = response.DecisionId,
                        Message = alreadyProcessed ? "Decision was already processed." : "Decision recorded."
                    };
                }

                default:
                    return new OfflineActionResultDto
                    {
                        ActionId = action.ActionId,
                        IdempotencyKey = action.IdempotencyKey,
                        Status = "Rejected",
                        ErrorCode = "UNSUPPORTED_ACTION_TYPE",
                        Message = $"Unsupported action type '{action.ActionType}'."
                    };
            }
        }
        catch (ServiceException ex)
        {
            return new OfflineActionResultDto
            {
                ActionId = action.ActionId,
                IdempotencyKey = action.IdempotencyKey,
                Status = "Rejected",
                ErrorCode = $"HTTP_{ex.StatusCode}",
                Message = ex.Message
            };
        }
        catch (JsonException ex)
        {
            return new OfflineActionResultDto
            {
                ActionId = action.ActionId,
                IdempotencyKey = action.IdempotencyKey,
                Status = "Rejected",
                ErrorCode = "INVALID_JSON",
                Message = $"Malformed action payload: {ex.Message}"
            };
        }
        catch (ValidationException ex)
        {
            return new OfflineActionResultDto
            {
                ActionId = action.ActionId,
                IdempotencyKey = action.IdempotencyKey,
                Status = "Rejected",
                ErrorCode = "VALIDATION_ERROR",
                Message = ex.Message
            };
        }
    }

    private static void ValidatePayload<T>(T payload) where T : notnull
    {
        var context = new ValidationContext(payload);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(payload, context, results, validateAllProperties: true))
        {
            var message = string.Join("; ", results.Select(r => r.ErrorMessage).Where(m => !string.IsNullOrWhiteSpace(m)));
            throw new ValidationException(message);
        }
    }
}

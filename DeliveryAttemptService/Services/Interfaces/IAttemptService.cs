using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;

namespace DeliveryAttemptService.Services.Interfaces;

public interface IAttemptService
{
    Task<(AttemptResponse Response, bool AlreadyProcessed)> RecordAttemptAsync(
        Guid driverId,
        RecordAttemptRequest request,
        CancellationToken ct = default);

    Task<AttemptResponse> GetAttemptByIdAsync(
        Guid attemptId,
        Guid callerId,
        string callerRole,
        CancellationToken ct = default);

    Task<List<AttemptResponse>> GetAttemptsByShipmentAsync(
        Guid shipmentId,
        Guid callerId,
        string callerRole,
        CancellationToken ct = default);
}

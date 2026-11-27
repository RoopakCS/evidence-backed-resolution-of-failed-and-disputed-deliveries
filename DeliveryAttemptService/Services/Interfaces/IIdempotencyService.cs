using DeliveryAttemptService.Models;

namespace DeliveryAttemptService.Services.Interfaces;

public interface IIdempotencyService
{
    Task<IdempotencyRecord?> GetAsync(string idempotencyKey, Guid userId, string actionType, CancellationToken ct = default);

    void StageRecord<T>(
        string idempotencyKey,
        Guid userId,
        string endpoint,
        string actionType,
        Guid resultEntityId,
        int statusCode,
        T response,
        string requestHash);

    string ComputeHash<T>(T payload);
}

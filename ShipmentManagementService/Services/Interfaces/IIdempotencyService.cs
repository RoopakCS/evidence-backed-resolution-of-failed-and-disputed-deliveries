using ShipmentManagementService.Models;

namespace ShipmentManagementService.Services.Interfaces;

public interface IIdempotencyService
{
    Task<IdempotencyRecord?> GetAsync(string key, Guid userId, string endpoint, CancellationToken ct = default);

    void StageRecord<T>(string key, Guid userId, string endpoint, int statusCode, T response);
}

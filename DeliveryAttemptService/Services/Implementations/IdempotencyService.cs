using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DeliveryAttemptService.Data;
using DeliveryAttemptService.Models;
using DeliveryAttemptService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAttemptService.Services.Implementations;

public class IdempotencyService : IIdempotencyService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly DeliveryAttemptDbContext _db;

    public IdempotencyService(DeliveryAttemptDbContext db)
    {
        _db = db;
    }

    public Task<IdempotencyRecord?> GetAsync(string idempotencyKey, Guid userId, string actionType, CancellationToken ct = default)
    {
        return _db.IdempotencyRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(
                r => r.IdempotencyKey == idempotencyKey && r.UserId == userId && r.ActionType == actionType,
                ct);
    }

    public void StageRecord<T>(
        string idempotencyKey,
        Guid userId,
        string endpoint,
        string actionType,
        Guid resultEntityId,
        int statusCode,
        T response,
        string requestHash)
    {
        _db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            IdempotencyKey = idempotencyKey,
            UserId = userId,
            Endpoint = endpoint,
            ActionType = actionType,
            ResultEntityId = resultEntityId,
            StatusCode = statusCode,
            ResponseBody = JsonSerializer.Serialize(response, JsonOptions),
            RequestHash = requestHash,
            CreatedAt = DateTime.UtcNow
        });
    }

    public string ComputeHash<T>(T payload)
    {
        var json = payload is string s ? s : JsonSerializer.Serialize(payload, JsonOptions);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexStringLower(bytes);
    }
}

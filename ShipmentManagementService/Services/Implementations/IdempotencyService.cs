using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ShipmentManagementService.Data;
using ShipmentManagementService.Models;
using ShipmentManagementService.Services.Interfaces;

namespace ShipmentManagementService.Services.Implementations;

public class IdempotencyService : IIdempotencyService
{
    private readonly ShipmentDbContext _db;

    public IdempotencyService(ShipmentDbContext db)
    {
        _db = db;
    }

    public Task<IdempotencyRecord?> GetAsync(string key, Guid userId, string endpoint, CancellationToken ct = default)
    {
        return _db.IdempotencyRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Key == key && r.UserId == userId && r.Endpoint == endpoint, ct);
    }

    public void StageRecord<T>(string key, Guid userId, string endpoint, int statusCode, T response)
    {
        _db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            Key = key,
            UserId = userId,
            Endpoint = endpoint,
            StatusCode = statusCode,
            ResponseBody = JsonSerializer.Serialize(response),
            CreatedAt = DateTime.UtcNow
        });
    }
}

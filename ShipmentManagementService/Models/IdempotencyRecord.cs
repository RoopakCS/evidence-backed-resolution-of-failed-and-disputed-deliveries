namespace ShipmentManagementService.Models;

/// <summary>
/// Stores client idempotency keys and their serialized responses so retries return the original result.
/// </summary>
public class IdempotencyRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Key { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public string Endpoint { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public string ResponseBody { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

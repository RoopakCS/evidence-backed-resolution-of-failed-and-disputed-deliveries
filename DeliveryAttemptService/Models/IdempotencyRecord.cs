namespace DeliveryAttemptService.Models;

public class IdempotencyRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string IdempotencyKey { get; set; } = string.Empty;

    public string ActionType { get; set; } = string.Empty;

    public Guid ResultEntityId { get; set; }

    public string RequestHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

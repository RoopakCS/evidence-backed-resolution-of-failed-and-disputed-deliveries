namespace DeliveryAttemptService.Models;

public class IdempotencyRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string IdempotencyKey { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public string Endpoint { get; set; } = string.Empty;

    public string ActionType { get; set; } = string.Empty;

    public Guid ResultEntityId { get; set; }

    public int StatusCode { get; set; }

    public string ResponseBody { get; set; } = string.Empty;

    public string RequestHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

namespace Contracts
{
    public class DeliveryAttemptCompleted
    {
        public required Guid EventId { get; init; }
        public required DateTime OccurredAt { get; init; }
        public required Guid AttemptId { get; init; }
        public required Guid ShipmentId { get; init; }
    }
}
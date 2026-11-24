namespace Contracts
{
    public class TicketStatusUpdated
    {
        public required Guid EventId { get; init; }
        public required DateTime OccurredAt { get; init; }
        public required Guid TicketId { get; init; }
        public required Guid ShipmentId { get; init; }
        public required string NewStatus { get; init; }
    }
}
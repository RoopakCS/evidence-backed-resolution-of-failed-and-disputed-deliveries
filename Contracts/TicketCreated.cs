namespace Contracts
{
    public class TicketCreated
    {
        public required Guid EventId { get; init; }
        public required DateTime OccurredAt { get; init; }
        public required Guid TicketId { get; init; }
        public required Guid ShipmentId { get; init; }
        public required string Source { get; init; }
        public required string Severity { get; init; }
    }
}
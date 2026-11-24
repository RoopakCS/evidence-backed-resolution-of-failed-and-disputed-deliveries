namespace Contracts
{
    public class ShipmentInstructionUpdated
    {
        public required Guid EventId { get; init; }
        public required DateTime OccurredAt { get; init; }
        public required Guid ShipmentId { get; init; }
        public required Guid InstructionId { get; init; }
        public required string State { get; init; }
    }
}
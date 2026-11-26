namespace TicketManagementService.Models.Enums;

public enum TicketEventType
{
    Created,
    Assigned,
    Reassigned,
    StatusChanged,
    EvidenceLinked,
    CommentAdded,
    Escalated,
    ResolutionRecorded,
    Closed,
    Reopened,
    RuleMatched,
    RuleDismissed
}

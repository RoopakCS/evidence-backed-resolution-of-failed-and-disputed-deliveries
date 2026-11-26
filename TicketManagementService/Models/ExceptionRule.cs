using TicketManagementService.Models.Enums;

namespace TicketManagementService.Models;

/// <summary>
/// Configurable exception rule evaluated against failed delivery attempts.
/// Rules are editable without redeploying; keep configuration validated and versionable.
/// </summary>
public class ExceptionRule
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Discriminator for rule evaluation logic (e.g. "ThresholdExceeded", "GeoFenceMismatch").
    /// </summary>
    public string RuleType { get; set; } = string.Empty;

    /// <summary>
    /// JSON-serialised threshold/configuration parameters for rule evaluation.
    /// </summary>
    public string Configuration { get; set; } = string.Empty;

    public TicketSeverity Severity { get; set; }

    public bool Active { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identity ID of the user who created this rule.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Identity ID of the user who last updated this rule.
    /// </summary>
    public Guid UpdatedBy { get; set; }
}

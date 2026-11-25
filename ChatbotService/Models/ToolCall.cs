using ChatbotService.Enums;

namespace ChatbotService.Models;

public class ToolCall
{
    public Guid Id { get; set; }

    public Guid MessageId { get; set; }

    public string ToolName { get; set; } = string.Empty;

    public string Arguments { get; set; } = string.Empty;

    public string? ResultSummary { get; set; }

    public ToolCallStatus Status { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? ErrorCode { get; set; }

    public Message Message { get; set; } = null!;
}
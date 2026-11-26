using System.Text.Json.Serialization;

namespace ChatbotService.DTOs.Groq;

public class GroqAssistantMessageDto
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("tool_calls")]
    public List<GroqToolCallDto>? ToolCalls { get; set; }
}
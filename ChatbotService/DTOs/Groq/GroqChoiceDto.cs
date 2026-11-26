using System.Text.Json.Serialization;

namespace ChatbotService.DTOs.Groq;

public class GroqChoiceDto
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("message")]
    public GroqAssistantMessageDto Message { get; set; } = new();

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}
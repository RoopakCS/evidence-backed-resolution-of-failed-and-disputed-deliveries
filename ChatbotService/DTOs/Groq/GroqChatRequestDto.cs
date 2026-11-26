using System.Text.Json.Serialization;
using ChatbotService.Tools.DTOs;

namespace ChatbotService.DTOs.Groq;

public class GroqChatRequestDto
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("messages")]
    public List<GroqMessageDto> Messages { get; set; } = [];

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.7;

    [JsonPropertyName("max_completion_tokens")]
    public int MaxCompletionTokens { get; set; } = 1024;

    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ChatToolDefinition>? Tools { get; set; }
}
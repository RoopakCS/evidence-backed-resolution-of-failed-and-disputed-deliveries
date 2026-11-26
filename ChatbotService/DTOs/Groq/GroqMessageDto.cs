using System.Text.Json.Serialization;

namespace ChatbotService.DTOs.Groq;

public class GroqMessageDto
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
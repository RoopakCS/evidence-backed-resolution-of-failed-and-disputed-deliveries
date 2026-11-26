using System.Text.Json.Serialization;

namespace ChatbotService.DTOs.Groq;

public class GroqToolCallDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("function")]
    public GroqToolCallFunctionDto Function { get; set; } = new();
}
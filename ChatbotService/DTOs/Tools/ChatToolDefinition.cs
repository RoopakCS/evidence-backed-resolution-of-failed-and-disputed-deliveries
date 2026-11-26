using System.Text.Json.Serialization;

namespace ChatbotService.Tools.DTOs;

public class ChatToolDefinition
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "function";

    [JsonPropertyName("function")]
    public ChatToolFunctionDefinition Function { get; set; } = new();
}
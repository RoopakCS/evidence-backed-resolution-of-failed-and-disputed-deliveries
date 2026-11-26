namespace ChatbotService.Configuration;

public class GroqSettings
{
    public string ApiKey { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = "https://api.groq.com/openai/v1/";

    public string Model { get; set; } = string.Empty;

    public double Temperature { get; set; } = 0.7;

    public int MaxTokens { get; set; } = 1024;
}
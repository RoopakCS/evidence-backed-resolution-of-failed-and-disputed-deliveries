namespace ChatbotService.Configuration;

public class ChatbotSettings
{
    public string Name { get; set; } = "Docket Assistant";

    public string Tone { get; set; } = "Friendly, professional, empathetic";

    public int MaxConversationMessages { get; set; } = 20;

    public int MaxResponseTokens { get; set; } = 1024;
}
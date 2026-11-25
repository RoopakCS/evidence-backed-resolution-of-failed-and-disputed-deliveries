namespace ChatbotService.DTOs;

public class SendMessageResponse
{
    public required MessageResponse UserMessage { get; set; }

    public required MessageResponse AssistantMessage { get; set; }
}
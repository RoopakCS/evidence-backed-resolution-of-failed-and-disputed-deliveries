namespace ChatbotService.DTOs;

public class ChatHistoryResponse
{
    public Guid ConversationId { get; set; }

    public IReadOnlyCollection<MessageResponse> Messages { get; set; } = Array.Empty<MessageResponse>();
}
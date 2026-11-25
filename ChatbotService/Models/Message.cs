using ChatbotService.Enums;

namespace ChatbotService.Models;

public class Message
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public MessageSender Sender { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Conversation Conversation { get; set; } = null!;
}
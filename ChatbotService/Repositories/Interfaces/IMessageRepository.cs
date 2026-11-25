using ChatbotService.Models;

namespace ChatbotService.Repositories.Interfaces;

public interface IMessageRepository
{
    Task<Message> CreateAsync(Message message);

    Task<List<Message>> GetByConversationIdAsync(Guid conversationId);
}
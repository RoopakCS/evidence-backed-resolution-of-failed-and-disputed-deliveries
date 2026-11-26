using ChatbotService.Models;

namespace ChatbotService.Repositories.Interfaces;

public interface IMessageRepository
{
    Task<Message> CreateAsync(
        Message message,
        CancellationToken cancellationToken = default);

    Task<List<Message>> GetByConversationIdAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);
}
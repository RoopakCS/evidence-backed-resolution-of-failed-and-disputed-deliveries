using ChatbotService.Models;

namespace ChatbotService.Repositories.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<Conversation?> GetByCustomerIdWithMessagesAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<Conversation> CreateAsync(
        Conversation conversation,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Conversation conversation,
        CancellationToken cancellationToken = default);
}
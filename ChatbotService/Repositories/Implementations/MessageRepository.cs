using ChatbotService.Data;
using ChatbotService.Models;
using ChatbotService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatbotService.Repositories.Implementations;

public class MessageRepository : IMessageRepository
{
    private readonly ChatbotDbContext _context;

    public MessageRepository(ChatbotDbContext context)
    {
        _context = context;
    }

    public async Task<Message> CreateAsync(
        Message message,
        CancellationToken cancellationToken = default)
    {
        await _context.Messages.AddAsync(message, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return message;
    }

    public async Task<List<Message>> GetByConversationIdAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
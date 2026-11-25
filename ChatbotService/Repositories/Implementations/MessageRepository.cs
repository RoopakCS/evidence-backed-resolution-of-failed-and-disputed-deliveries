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

    public async Task<Message> CreateAsync(Message message)
    {
        await _context.Messages.AddAsync(message);

        await _context.SaveChangesAsync();

        return message;
    }

    public async Task<List<Message>> GetByConversationIdAsync(Guid conversationId)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }
}
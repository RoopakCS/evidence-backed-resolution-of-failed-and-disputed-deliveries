using ChatbotService.DTOs;
using ChatbotService.Enums;
using ChatbotService.Models;
using ChatbotService.Repositories.Interfaces;
using ChatbotService.Services.Interfaces;

namespace ChatbotService.Services.Implementations;

public class ChatService : IChatService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;

    public ChatService(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task<SendMessageResponse> SendMessageAsync(
        Guid customerId,
        SendMessageRequest request)
    {
        // Find the customer's existing conversation
        var conversation = await _conversationRepository
            .GetByCustomerIdAsync(customerId);

        // Create a conversation if one doesn't exist
        if (conversation == null)
        {
            conversation = new Conversation
            {
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            conversation = await _conversationRepository
                .CreateAsync(conversation);
        }

        // Save the customer's message
        var userMessage = new Message
        {
            ConversationId = conversation.Id,
            Sender = MessageSender.User,
            Content = request.Message,
            CreatedAt = DateTime.UtcNow
        };

        userMessage = await _messageRepository.CreateAsync(userMessage);

        // Temporary placeholder response
        var assistantMessage = new Message
        {
            ConversationId = conversation.Id,
            Sender = MessageSender.Assistant,
            Content = "Hello! I'm Docket's assistant. AI integration is coming soon!",
            CreatedAt = DateTime.UtcNow
        };

        assistantMessage = await _messageRepository
            .CreateAsync(assistantMessage);

        // Update conversation's last activity time
        conversation.UpdatedAt = DateTime.UtcNow;

        await _conversationRepository.UpdateAsync(conversation);

        // Return both messages
        return new SendMessageResponse
        {
            UserMessage = MapToMessageResponse(userMessage),
            AssistantMessage = MapToMessageResponse(assistantMessage)
        };
    }

    public async Task<ChatHistoryResponse> GetHistoryAsync(Guid customerId)
    {
        var conversation = await _conversationRepository
            .GetByCustomerIdWithMessagesAsync(customerId);

        // Customer has no conversation yet
        if (conversation == null)
        {
            return new ChatHistoryResponse
            {
                ConversationId = Guid.Empty,
                Messages = []
            };
        }

        return new ChatHistoryResponse
        {
            ConversationId = conversation.Id,
            Messages = conversation.Messages
                .OrderBy(m => m.CreatedAt)
                .Select(MapToMessageResponse)
                .ToList()
        };
    }

    private static MessageResponse MapToMessageResponse(Message message)
    {
        return new MessageResponse
        {
            Id = message.Id,
            Sender = message.Sender,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            Citations = []
        };
    }
}
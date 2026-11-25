using ChatbotService.DTOs;

namespace ChatbotService.Services.Interfaces;

public interface IChatService
{
    Task<SendMessageResponse> SendMessageAsync(
        Guid customerId,
        SendMessageRequest request);

    Task<ChatHistoryResponse> GetHistoryAsync(Guid customerId);
}
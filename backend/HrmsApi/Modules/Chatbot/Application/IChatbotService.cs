using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HrmsApi.Modules.Chatbot.Application.DTOs;
using HrmsApi.Modules.Chatbot.Domain;

namespace HrmsApi.Modules.Chatbot.Application
{
    public interface IChatbotService
    {
        Task<ChatbotResponseDto> ProcessQueryAsync(ChatbotQueryDto query, string userRole);
        Task<IEnumerable<ChatbotIntent>> GetAllIntentsAsync();
        Task<ChatbotIntent> GetIntentByIdAsync(Guid id);
        Task<ChatbotIntent> CreateIntentAsync(ChatbotIntent intent);
        Task<bool> UpdateIntentAsync(ChatbotIntent intent);
        Task<bool> DeleteIntentAsync(Guid id);
        Task<IEnumerable<ChatbotMessage>> GetConversationHistoryAsync(Guid conversationId);
        Task<IEnumerable<ChatbotConversation>> GetUserConversationsAsync(Guid employeeId);
        Task<bool> TrainChatbotAsync();
    }
}

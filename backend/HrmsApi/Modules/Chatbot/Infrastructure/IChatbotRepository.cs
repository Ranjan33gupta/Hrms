using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HrmsApi.Modules.Chatbot.Domain;

namespace HrmsApi.Modules.Chatbot.Infrastructure
{
    public interface IChatbotRepository
    {
        // Intent management
        Task<IEnumerable<ChatbotIntent>> GetAllIntentsAsync();
        Task<ChatbotIntent> GetIntentByIdAsync(Guid id);
        Task<ChatbotIntent> GetIntentByNameAsync(string name);
        Task<ChatbotIntent> CreateIntentAsync(ChatbotIntent intent);
        Task<bool> UpdateIntentAsync(ChatbotIntent intent);
        Task<bool> DeleteIntentAsync(Guid id);

        // Training phrases
        Task<IEnumerable<ChatbotTrainingPhrase>> GetTrainingPhrasesByIntentIdAsync(Guid intentId);
        Task<ChatbotTrainingPhrase> AddTrainingPhraseAsync(ChatbotTrainingPhrase phrase);
        Task<bool> DeleteTrainingPhraseAsync(Guid id);

        // Responses
        Task<IEnumerable<ChatbotResponse>> GetResponsesByIntentIdAsync(Guid intentId);
        Task<ChatbotResponse> AddResponseAsync(ChatbotResponse response);
        Task<bool> DeleteResponseAsync(Guid id);

        // Conversation tracking
        Task<ChatbotConversation> CreateConversationAsync(Guid? employeeId);
        Task<ChatbotConversation> GetConversationByIdAsync(Guid id);
        Task<IEnumerable<ChatbotConversation>> GetConversationsByEmployeeIdAsync(Guid employeeId);
        Task<bool> EndConversationAsync(Guid id);

        // Messages
        Task<ChatbotMessage> AddMessageAsync(ChatbotMessage message);
        Task<IEnumerable<ChatbotMessage>> GetMessagesByConversationIdAsync(Guid conversationId);
    }
}

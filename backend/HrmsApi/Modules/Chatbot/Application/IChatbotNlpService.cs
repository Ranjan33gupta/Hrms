using System;
using System.Threading.Tasks;
using HrmsApi.Modules.Chatbot.Domain;

namespace HrmsApi.Modules.Chatbot.Application
{
    public interface IChatbotNlpService
    {
        Task<(ChatbotIntent intent, double confidence)> RecognizeIntentAsync(string query, string userRole);
        Task<string> GenerateResponseAsync(ChatbotIntent intent, string query, Guid? employeeId);
        Task<bool> TrainModelAsync();
        Task<string> ProcessVoiceCommandAsync(byte[] audioData, string userRole, Guid? employeeId);
    }
}

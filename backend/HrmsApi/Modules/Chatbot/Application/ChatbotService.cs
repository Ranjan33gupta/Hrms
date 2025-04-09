using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HrmsApi.Modules.Chatbot.Application.DTOs;
using HrmsApi.Modules.Chatbot.Domain;
using HrmsApi.Modules.Chatbot.Infrastructure;
using Microsoft.Extensions.Logging;

namespace HrmsApi.Modules.Chatbot.Application
{
    public class ChatbotService : IChatbotService
    {
        private readonly IChatbotRepository _chatbotRepository;
        private readonly IChatbotNlpService _nlpService;
        private readonly ILogger<ChatbotService> _logger;

        public ChatbotService(
            IChatbotRepository chatbotRepository,
            IChatbotNlpService nlpService,
            ILogger<ChatbotService> logger)
        {
            _chatbotRepository = chatbotRepository;
            _nlpService = nlpService;
            _logger = logger;
        }

        public async Task<ChatbotResponseDto> ProcessQueryAsync(ChatbotQueryDto query, string userRole)
        {
            try
            {
                // Create or get conversation
                ChatbotConversation conversation;
                if (query.ConversationId.HasValue)
                {
                    conversation = await _chatbotRepository.GetConversationByIdAsync(query.ConversationId.Value);
                    if (conversation == null)
                    {
                        conversation = await _chatbotRepository.CreateConversationAsync(query.EmployeeId);
                    }
                }
                else
                {
                    conversation = await _chatbotRepository.CreateConversationAsync(query.EmployeeId);
                }

                // Process the query
                string userMessage = query.Query;
                
                // Handle voice commands if applicable
                if (query.IsVoiceCommand && query.VoiceData != null && query.VoiceData.Length > 0)
                {
                    userMessage = await _nlpService.ProcessVoiceCommandAsync(query.VoiceData, userRole, query.EmployeeId);
                }

                // Save user message
                var userChatMessage = new ChatbotMessage
                {
                    ConversationId = conversation.Id,
                    Message = userMessage,
                    IsFromUser = true,
                    Timestamp = DateTime.UtcNow
                };
                await _chatbotRepository.AddMessageAsync(userChatMessage);

                // Recognize intent
                var (intent, confidence) = await _nlpService.RecognizeIntentAsync(userMessage, userRole);

                // Generate response
                string responseText = await _nlpService.GenerateResponseAsync(intent, userMessage, query.EmployeeId);

                // Save bot response
                var botChatMessage = new ChatbotMessage
                {
                    ConversationId = conversation.Id,
                    Message = responseText,
                    IsFromUser = false,
                    IntentId = intent?.Id,
                    ConfidenceScore = confidence,
                    Timestamp = DateTime.UtcNow
                };
                await _chatbotRepository.AddMessageAsync(botChatMessage);

                // Create response DTO
                var responseDto = new ChatbotResponseDto
                {
                    Response = responseText,
                    IntentName = intent?.Name,
                    Confidence = confidence,
                    ConversationId = conversation.Id,
                    Action = null,
                    ApiEndpoint = intent?.ApiEndpoint,
                    RouteDestination = intent?.RouteDestination,
                    RequiresAuth = intent?.RequiresAuth ?? false
                };

                return responseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chatbot query");
                return new ChatbotResponseDto
                {
                    Response = "I'm sorry, I encountered an error processing your request. Please try again later.",
                    IntentName = "error",
                    Confidence = 0,
                    ConversationId = query.ConversationId ?? Guid.Empty
                };
            }
        }

        public async Task<IEnumerable<ChatbotIntent>> GetAllIntentsAsync()
        {
            return await _chatbotRepository.GetAllIntentsAsync();
        }

        public async Task<ChatbotIntent> GetIntentByIdAsync(Guid id)
        {
            return await _chatbotRepository.GetIntentByIdAsync(id);
        }

        public async Task<ChatbotIntent> CreateIntentAsync(ChatbotIntent intent)
        {
            var result = await _chatbotRepository.CreateIntentAsync(intent);
            await _nlpService.TrainModelAsync();
            return result;
        }

        public async Task<bool> UpdateIntentAsync(ChatbotIntent intent)
        {
            var result = await _chatbotRepository.UpdateIntentAsync(intent);
            await _nlpService.TrainModelAsync();
            return result;
        }

        public async Task<bool> DeleteIntentAsync(Guid id)
        {
            var result = await _chatbotRepository.DeleteIntentAsync(id);
            await _nlpService.TrainModelAsync();
            return result;
        }

        public async Task<IEnumerable<ChatbotMessage>> GetConversationHistoryAsync(Guid conversationId)
        {
            return await _chatbotRepository.GetMessagesByConversationIdAsync(conversationId);
        }

        public async Task<IEnumerable<ChatbotConversation>> GetUserConversationsAsync(Guid employeeId)
        {
            return await _chatbotRepository.GetConversationsByEmployeeIdAsync(employeeId);
        }

        public async Task<bool> TrainChatbotAsync()
        {
            return await _nlpService.TrainModelAsync();
        }
    }
}

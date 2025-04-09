using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HrmsApi.Modules.Chatbot.Application.DTOs;
using HrmsApi.Modules.Chatbot.Domain.Entities;
using HrmsApi.Modules.Chatbot.Infrastructure.Repositories;

namespace HrmsApi.Modules.Chatbot.Application.Services
{
    public class ChatbotService
    {
        private readonly IntentRecognitionService _intentRecognitionService;
        private readonly ChatbotIntentRepository _intentRepository;
        private readonly ChatbotConversationRepository _conversationRepository;

        public ChatbotService(
            IntentRecognitionService intentRecognitionService,
            ChatbotIntentRepository intentRepository,
            ChatbotConversationRepository conversationRepository)
        {
            _intentRecognitionService = intentRecognitionService;
            _intentRepository = intentRepository;
            _conversationRepository = conversationRepository;
        }

        public async Task<ChatbotResponseDto> ProcessQueryAsync(ChatbotQueryDto query, string userRole)
        {
            // Get the user message from either Query or Message property
            string userMessage = !string.IsNullOrEmpty(query.Query) ? query.Query : query.Message;
            
            if (string.IsNullOrEmpty(userMessage))
            {
                return new ChatbotResponseDto 
                { 
                    Response = "I didn't receive a message. How can I help you?",
                    Message = "I didn't receive a message. How can I help you?"
                };
            }

            // Get or create conversation
            Guid conversationId = query.ConversationId ?? Guid.NewGuid();
            ChatbotConversation conversation = null;
            
            if (query.ConversationId.HasValue)
            {
                conversation = await _conversationRepository.GetWithMessagesAsync(conversationId);
            }
            
            if (conversation == null)
            {
                conversation = new ChatbotConversation
                {
                    Id = conversationId,
                    EmployeeId = query.EmployeeId,
                    StartedAt = DateTime.UtcNow,
                    LastMessageAt = DateTime.UtcNow,
                    IsActive = true
                };
                
                await _conversationRepository.AddAsync(conversation);
            }
            else
            {
                conversation.LastMessageAt = DateTime.UtcNow;
                await _conversationRepository.UpdateAsync(conversation);
            }
            
            // Save user message
            var userMessageEntity = new ChatbotMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                Content = userMessage,
                Timestamp = DateTime.UtcNow,
                IsFromUser = true
            };
            
            conversation.Messages.Add(userMessageEntity);
            
            // Recognize intent
            var (intentName, confidence, entities) = await _intentRecognitionService.RecognizeIntentAsync(userMessage);
            
            // Get response based on intent
            string responseText = "I'm not sure I understand. Could you rephrase that?";
            
            if (intentName != "none")
            {
                var intent = await _intentRepository.GetByNameAsync(intentName);
                if (intent != null)
                {
                    responseText = intent.ResponseTemplate;
                    
                    // Replace entity placeholders in the response
                    foreach (var entity in entities)
                    {
                        responseText = responseText.Replace("{" + entity.Key + "}", entity.Value);
                    }
                    
                    // TODO: If the intent has an API endpoint, call it to get dynamic data
                    // This would be implemented in a more advanced version
                }
            }
            
            // Save bot response
            var botMessageEntity = new ChatbotMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                Content = responseText,
                Timestamp = DateTime.UtcNow,
                IsFromUser = false
            };
            
            conversation.Messages.Add(botMessageEntity);
            await _conversationRepository.UpdateAsync(conversation);
            
            // Return response
            return new ChatbotResponseDto
            {
                Response = responseText,
                Message = responseText,
                ConversationId = conversationId,
                Intent = intentName,
                Confidence = confidence,
                RequiresAuth = intentName != "none" ? 
                    (await _intentRepository.GetByNameAsync(intentName))?.RequiresAuth ?? false : 
                    false
            };
        }
    }
}

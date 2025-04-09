using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HrmsApi.Modules.Chatbot.Domain;
using HrmsApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HrmsApi.Modules.Chatbot.Infrastructure
{
    public class ChatbotRepository : IChatbotRepository
    {
        private readonly HrmsDbContext _context;

        public ChatbotRepository(HrmsDbContext context)
        {
            _context = context;
        }

        // Intent management
        public async Task<IEnumerable<ChatbotIntent>> GetAllIntentsAsync()
        {
            return await _context.ChatbotIntents
                .Include(i => i.TrainingPhrases)
                .Include(i => i.Responses)
                .ToListAsync();
        }

        public async Task<ChatbotIntent> GetIntentByIdAsync(Guid id)
        {
            return await _context.ChatbotIntents
                .Include(i => i.TrainingPhrases)
                .Include(i => i.Responses)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<ChatbotIntent> GetIntentByNameAsync(string name)
        {
            return await _context.ChatbotIntents
                .Include(i => i.TrainingPhrases)
                .Include(i => i.Responses)
                .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
        }

        public async Task<ChatbotIntent> CreateIntentAsync(ChatbotIntent intent)
        {
            _context.ChatbotIntents.Add(intent);
            await _context.SaveChangesAsync();
            return intent;
        }

        public async Task<bool> UpdateIntentAsync(ChatbotIntent intent)
        {
            _context.Entry(intent).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteIntentAsync(Guid id)
        {
            var intent = await _context.ChatbotIntents.FindAsync(id);
            if (intent == null)
                return false;

            _context.ChatbotIntents.Remove(intent);
            return await _context.SaveChangesAsync() > 0;
        }

        // Training phrases
        public async Task<IEnumerable<ChatbotTrainingPhrase>> GetTrainingPhrasesByIntentIdAsync(Guid intentId)
        {
            return await _context.ChatbotTrainingPhrases
                .Where(p => p.IntentId == intentId)
                .ToListAsync();
        }

        public async Task<ChatbotTrainingPhrase> AddTrainingPhraseAsync(ChatbotTrainingPhrase phrase)
        {
            _context.ChatbotTrainingPhrases.Add(phrase);
            await _context.SaveChangesAsync();
            return phrase;
        }

        public async Task<bool> DeleteTrainingPhraseAsync(Guid id)
        {
            var phrase = await _context.ChatbotTrainingPhrases.FindAsync(id);
            if (phrase == null)
                return false;

            _context.ChatbotTrainingPhrases.Remove(phrase);
            return await _context.SaveChangesAsync() > 0;
        }

        // Responses
        public async Task<IEnumerable<ChatbotResponse>> GetResponsesByIntentIdAsync(Guid intentId)
        {
            return await _context.ChatbotResponses
                .Where(r => r.IntentId == intentId)
                .OrderByDescending(r => r.Priority)
                .ToListAsync();
        }

        public async Task<ChatbotResponse> AddResponseAsync(ChatbotResponse response)
        {
            _context.ChatbotResponses.Add(response);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<bool> DeleteResponseAsync(Guid id)
        {
            var response = await _context.ChatbotResponses.FindAsync(id);
            if (response == null)
                return false;

            _context.ChatbotResponses.Remove(response);
            return await _context.SaveChangesAsync() > 0;
        }

        // Conversation tracking
        public async Task<ChatbotConversation> CreateConversationAsync(Guid? employeeId)
        {
            var conversation = new ChatbotConversation
            {
                EmployeeId = employeeId,
                StartedAt = DateTime.UtcNow
            };

            _context.ChatbotConversations.Add(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task<ChatbotConversation> GetConversationByIdAsync(Guid id)
        {
            return await _context.ChatbotConversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<ChatbotConversation>> GetConversationsByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.ChatbotConversations
                .Where(c => c.EmployeeId == employeeId)
                .OrderByDescending(c => c.StartedAt)
                .ToListAsync();
        }

        public async Task<bool> EndConversationAsync(Guid id)
        {
            var conversation = await _context.ChatbotConversations.FindAsync(id);
            if (conversation == null)
                return false;

            conversation.EndedAt = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }

        // Messages
        public async Task<ChatbotMessage> AddMessageAsync(ChatbotMessage message)
        {
            _context.ChatbotMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<IEnumerable<ChatbotMessage>> GetMessagesByConversationIdAsync(Guid conversationId)
        {
            return await _context.ChatbotMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }
    }
}

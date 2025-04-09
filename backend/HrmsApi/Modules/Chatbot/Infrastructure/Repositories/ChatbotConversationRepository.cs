using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HrmsApi.Modules.Chatbot.Domain.Entities;
using HrmsApi.Modules.Chatbot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HrmsApi.Modules.Chatbot.Infrastructure.Repositories
{
    public class ChatbotConversationRepository
    {
        private readonly ChatbotDbContext _dbContext;

        public ChatbotConversationRepository(ChatbotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ChatbotConversation> GetWithMessagesAsync(Guid id)
        {
            return await _dbContext.Conversations
                .Include(c => c.Messages.OrderBy(m => m.Timestamp))
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ChatbotConversation> GetActiveConversationForEmployeeAsync(Guid employeeId)
        {
            return await _dbContext.Conversations
                .Include(c => c.Messages.OrderBy(m => m.Timestamp))
                .FirstOrDefaultAsync(c => c.EmployeeId == employeeId && c.IsActive);
        }

        public async Task<List<ChatbotConversation>> GetConversationHistoryForEmployeeAsync(Guid employeeId, int limit = 10)
        {
            return await _dbContext.Conversations
                .Include(c => c.Messages.OrderBy(m => m.Timestamp))
                .Where(c => c.EmployeeId == employeeId)
                .OrderByDescending(c => c.LastMessageAt)
                .Take(limit)
                .ToListAsync();
        }
        
        public async Task<ChatbotConversation> AddAsync(ChatbotConversation entity)
        {
            await _dbContext.Conversations.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<ChatbotConversation> UpdateAsync(ChatbotConversation entity)
        {
            _dbContext.Conversations.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}

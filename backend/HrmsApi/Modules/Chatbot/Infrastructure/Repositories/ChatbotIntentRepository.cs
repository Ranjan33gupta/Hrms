using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HrmsApi.Modules.Chatbot.Domain.Entities;
using HrmsApi.Modules.Chatbot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HrmsApi.Modules.Chatbot.Infrastructure.Repositories
{
    public class ChatbotIntentRepository
    {
        private readonly ChatbotDbContext _dbContext;

        public ChatbotIntentRepository(ChatbotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ChatbotIntent>> GetAllWithTrainingPhrasesAsync()
        {
            return await _dbContext.Intents
                .Include(i => i.TrainingPhrases)
                .ToListAsync();
        }

        public async Task<ChatbotIntent> GetByNameAsync(string name)
        {
            return await _dbContext.Intents
                .Include(i => i.TrainingPhrases)
                .Include(i => i.Entities)
                .FirstOrDefaultAsync(i => i.Name == name);
        }

        public async Task<List<ChatbotIntent>> GetByCategoryAsync(string category)
        {
            return await _dbContext.Intents
                .Include(i => i.TrainingPhrases)
                .Where(i => i.Category == category)
                .ToListAsync();
        }
        
        public async Task<ChatbotIntent> AddAsync(ChatbotIntent entity)
        {
            await _dbContext.Intents.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<ChatbotIntent> UpdateAsync(ChatbotIntent entity)
        {
            _dbContext.Intents.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}

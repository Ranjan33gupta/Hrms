using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HrmsApi.Modules.MoodChanger.Domain.Entities;
using HrmsApi.Modules.MoodChanger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HrmsApi.Modules.MoodChanger.Infrastructure.Repositories
{
    public class MoodEntryRepository
    {
        private readonly MoodChangerDbContext _dbContext;

        public MoodEntryRepository(MoodChangerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MoodEntry>> GetByEmployeeIdAsync(Guid employeeId, int limit = 10)
        {
            return await _dbContext.MoodEntries
                .Where(m => m.EmployeeId == employeeId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<MoodEntry>> GetRecentEntriesAsync(int limit = 50)
        {
            return await _dbContext.MoodEntries
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<MoodEntry> GetLatestByEmployeeIdAsync(Guid employeeId)
        {
            return await _dbContext.MoodEntries
                .Where(m => m.EmployeeId == employeeId)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<MoodEntry> AddAsync(MoodEntry entity)
        {
            await _dbContext.MoodEntries.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<MoodEntry> UpdateAsync(MoodEntry entity)
        {
            _dbContext.MoodEntries.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}

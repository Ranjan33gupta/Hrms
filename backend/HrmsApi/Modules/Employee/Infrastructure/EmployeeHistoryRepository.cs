using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Employee.Application.Interfaces;
using HrmsApi.Modules.Employee.Domain;
using HrmsApi.Modules.Employee.Application.DTOs;

namespace HrmsApi.Modules.Employee.Infrastructure
{
    public class EmployeeHistoryRepository : IEmployeeHistoryRepository
    {
        private readonly HrmsDbContext _context;

        public EmployeeHistoryRepository(HrmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeHistory>> GetAllAsync()
        {
            return await _context.EmployeeHistories.ToListAsync();
        }

        public async Task<EmployeeHistory> GetByIdAsync(Guid id)
        {
            return await _context.EmployeeHistories.FindAsync(id);
        }

        public async Task<IEnumerable<EmployeeHistory>> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.EmployeeHistories
                .Where(eh => eh.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<EmployeeHistory> CreateAsync(EmployeeHistory employeeHistory)
        {
            _context.EmployeeHistories.Add(employeeHistory);
            await _context.SaveChangesAsync();
            return employeeHistory;
        }

        public async Task<EmployeeHistory> UpdateAsync(EmployeeHistory employeeHistory)
        {
            _context.Entry(employeeHistory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return employeeHistory;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var employeeHistory = await _context.EmployeeHistories.FindAsync(id);
            if (employeeHistory == null)
                return false;

            _context.EmployeeHistories.Remove(employeeHistory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddHistoryLogAsync(Guid employeeId, string employeeName, EmployeeChangeDetail logEntry)
        {
            try
            {
                // Find existing history record for the employee or create a new one
                var employeeHistory = await _context.EmployeeHistories
                    .FirstOrDefaultAsync(eh => eh.EmployeeId == employeeId);

                if (employeeHistory == null)
                {
                    employeeHistory = new EmployeeHistory
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = employeeId,
                        EmployeeName = employeeName,
                        EmployeeChangeDetails = new Dictionary<string, List<EmployeeChangeDetail>>()
                    };
                    _context.EmployeeHistories.Add(employeeHistory);
                }

                // Get the current UTC time with proper Kind and convert to ISO 8601 string
                var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                var timeKey = now.ToString("o"); // ISO 8601 format

                // Add the log entry
                if (!employeeHistory.EmployeeChangeDetails.ContainsKey(timeKey))
                {
                    employeeHistory.EmployeeChangeDetails[timeKey] = new List<EmployeeChangeDetail>();
                }

                employeeHistory.EmployeeChangeDetails[timeKey].Add(logEntry);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<FlattenedEmployeeHistoryDTO>> GetFlattenedHistoryByEmployeeIdAsync(Guid employeeId)
        {
            var histories = await _context.EmployeeHistories
                .Where(eh => eh.EmployeeId == employeeId)
                .ToListAsync();

            var flattenedHistories = new List<FlattenedEmployeeHistoryDTO>();

            foreach (var history in histories)
            {
                if (history.EmployeeChangeDetails == null)
                {
                    continue;
                }

                foreach (var changeEntry in history.EmployeeChangeDetails)
                {
                    var changeDateString = changeEntry.Key;
                    var changes = changeEntry.Value;

                    if (changes == null)
                    {
                        continue;
                    }

                    // Parse the ISO 8601 date string back to DateTime
                    if (DateTime.TryParse(changeDateString, out DateTime changeDate))
                    {
                        foreach (var change in changes)
                        {
                            flattenedHistories.Add(new FlattenedEmployeeHistoryDTO
                            {
                                Id = history.Id,
                                EmployeeId = history.EmployeeId,
                                EmployeeName = history.EmployeeName,
                                ChangeDate = changeDate,
                                Action = change.Action,
                                FieldChanged = change.FieldChanged,
                                OldValue = change.OldValue,
                                NewValue = change.NewValue,
                                ChangedBy = changeDate > history.CreatedAt ? history.UpdatedBy : history.CreatedBy
                            });
                        }
                    }
                }
            }

            // Return sorted by date, most recent first
            return flattenedHistories.OrderByDescending(h => h.ChangeDate);
        }

        public async Task<IEnumerable<FlattenedEmployeeHistoryDTO>> SearchHistoryAsync(
            Guid? employeeId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? actionType = null,
            string? fieldName = null)
        {
            // Get all histories first
            var query = _context.EmployeeHistories.AsQueryable();

            // Filter by employee if specified
            if (employeeId.HasValue)
            {
                query = query.Where(eh => eh.EmployeeId == employeeId.Value);
            }

            var histories = await query.ToListAsync();
            var flattenedHistories = new List<FlattenedEmployeeHistoryDTO>();

            foreach (var history in histories)
            {
                if (history.EmployeeChangeDetails == null)
                    continue;

                foreach (var changeEntry in history.EmployeeChangeDetails)
                {
                    var changeDateString = changeEntry.Key;

                    // Parse the ISO 8601 date string back to DateTime
                    if (!DateTime.TryParse(changeDateString, out DateTime changeDate))
                        continue;

                    // Filter by date range if specified
                    if (startDate.HasValue && changeDate < startDate.Value)
                        continue;

                    if (endDate.HasValue && changeDate > endDate.Value)
                        continue;

                    var changes = changeEntry.Value;

                    foreach (var change in changes)
                    {
                        // Filter by action type if specified
                        if (!string.IsNullOrEmpty(actionType) && !change.Action.Equals(actionType, StringComparison.OrdinalIgnoreCase))
                            continue;

                        // Filter by field name if specified
                        if (!string.IsNullOrEmpty(fieldName) && !change.FieldChanged.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                            continue;

                        flattenedHistories.Add(new FlattenedEmployeeHistoryDTO
                        {
                            Id = history.Id,
                            EmployeeId = history.EmployeeId,
                            EmployeeName = history.EmployeeName,
                            ChangeDate = changeDate,
                            Action = change.Action,
                            FieldChanged = change.FieldChanged,
                            OldValue = change.OldValue,
                            NewValue = change.NewValue,
                            ChangedBy = changeDate > history.CreatedAt ? history.UpdatedBy : history.CreatedBy
                        });
                    }
                }
            }

            // Return sorted by date, most recent first
            return flattenedHistories.OrderByDescending(h => h.ChangeDate);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Dashboard.Domain;
using HrmsApi.Modules.Dashboard.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HrmsApi.Modules.Dashboard.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoodController : ControllerBase
    {
        private readonly HrmsDbContext _context;

        public MoodController(HrmsDbContext context)
        {
            _context = context;
        }

        // GET: api/Mood
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MoodEntryDTO>>> GetMoodEntries()
        {
            try
            {
                var entries = await _context.MoodEntries.ToListAsync();
                var employees = await _context.Employees.ToListAsync();
                
                var result = entries.Select(entry => new MoodEntryDTO
                {
                    Id = entry.Id,
                    EmployeeId = entry.EmployeeId,
                    EmployeeName = employees.FirstOrDefault(e => e.Id == entry.EmployeeId)?.FullName ?? "Unknown",
                    EntryDate = entry.EntryDate,
                    Mood = entry.Mood,
                    Comment = entry.Comment,
                    SentimentScore = entry.SentimentScore,
                    CreatedAt = entry.CreatedAt
                }).ToList();
                
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Mood/Employee/5
        [HttpGet("Employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<MoodEntryDTO>>> GetMoodEntriesByEmployee(Guid employeeId)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(employeeId);
                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                var entries = await _context.MoodEntries
                    .Where(m => m.EmployeeId == employeeId)
                    .OrderByDescending(m => m.EntryDate)
                    .ToListAsync();
                
                var result = entries.Select(entry => new MoodEntryDTO
                {
                    Id = entry.Id,
                    EmployeeId = entry.EmployeeId,
                    EmployeeName = employee.FullName,
                    EntryDate = entry.EntryDate,
                    Mood = entry.Mood,
                    Comment = entry.Comment,
                    SentimentScore = entry.SentimentScore,
                    CreatedAt = entry.CreatedAt
                }).ToList();
                
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Mood
        [HttpPost]
        public async Task<ActionResult<MoodEntryDTO>> CreateMoodEntry([FromBody] CreateMoodEntryDTO moodDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var employee = await _context.Employees.FindAsync(moodDto.EmployeeId);
                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                // Calculate sentiment score (simple implementation)
                double? sentimentScore = AnalyzeSentiment(moodDto.Comment);

                var moodEntry = new MoodEntry
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = moodDto.EmployeeId,
                    EntryDate = DateTime.Now,
                    Mood = moodDto.Mood,
                    Comment = moodDto.Comment,
                    SentimentScore = sentimentScore,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                _context.MoodEntries.Add(moodEntry);
                await _context.SaveChangesAsync();

                var result = new MoodEntryDTO
                {
                    Id = moodEntry.Id,
                    EmployeeId = moodEntry.EmployeeId,
                    EmployeeName = employee.FullName,
                    EntryDate = moodEntry.EntryDate,
                    Mood = moodEntry.Mood,
                    Comment = moodEntry.Comment,
                    SentimentScore = moodEntry.SentimentScore,
                    CreatedAt = moodEntry.CreatedAt
                };

                return CreatedAtAction(nameof(GetMoodEntries), new { id = moodEntry.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Mood/Analytics
        [HttpGet("Analytics")]
        public async Task<ActionResult<MoodAnalyticsDTO>> GetMoodAnalytics()
        {
            try
            {
                var entries = await _context.MoodEntries
                    .OrderByDescending(m => m.EntryDate)
                    .ToListAsync();
                
                if (!entries.Any())
                {
                    return new MoodAnalyticsDTO
                    {
                        AverageMood = 0,
                        TotalEntries = 0,
                        MoodDistribution = new Dictionary<string, int>(),
                        RecentTrend = "No data available"
                    };
                }

                // Calculate average mood
                double averageMood = entries.Average(e => (int)e.Mood);
                
                // Calculate mood distribution
                var moodDistribution = entries
                    .GroupBy(e => e.Mood)
                    .ToDictionary(
                        g => g.Key.ToString(), 
                        g => g.Count()
                    );
                
                // Determine recent trend (last 7 days vs previous 7 days)
                var last7Days = entries
                    .Where(e => e.EntryDate >= DateTime.Now.AddDays(-7))
                    .ToList();
                
                var previous7Days = entries
                    .Where(e => e.EntryDate < DateTime.Now.AddDays(-7) && e.EntryDate >= DateTime.Now.AddDays(-14))
                    .ToList();
                
                string recentTrend = "Stable";
                if (last7Days.Any() && previous7Days.Any())
                {
                    double last7DaysAvg = last7Days.Average(e => (int)e.Mood);
                    double previous7DaysAvg = previous7Days.Average(e => (int)e.Mood);
                    
                    double difference = last7DaysAvg - previous7DaysAvg;
                    
                    if (difference > 0.5)
                        recentTrend = "Improving";
                    else if (difference < -0.5)
                        recentTrend = "Declining";
                    else
                        recentTrend = "Stable";
                }
                
                return new MoodAnalyticsDTO
                {
                    AverageMood = Math.Round(averageMood, 2),
                    TotalEntries = entries.Count,
                    MoodDistribution = moodDistribution,
                    RecentTrend = recentTrend
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Simple sentiment analysis (can be replaced with more sophisticated NLP)
        private double? AnalyzeSentiment(string? comment)
        {
            if (string.IsNullOrEmpty(comment))
                return null;
                
            // List of positive and negative words (very simple implementation)
            var positiveWords = new[] { "happy", "good", "great", "excellent", "amazing", "wonderful", "fantastic", "pleased", "joy", "love", "like", "positive" };
            var negativeWords = new[] { "sad", "bad", "terrible", "awful", "horrible", "disappointed", "upset", "angry", "hate", "dislike", "negative", "stress", "stressed" };
            
            comment = comment.ToLower();
            
            // Count positive and negative words
            int positiveCount = positiveWords.Sum(word => comment.Split(' ').Count(w => w.Contains(word)));
            int negativeCount = negativeWords.Sum(word => comment.Split(' ').Count(w => w.Contains(word)));
            
            // Calculate sentiment score (-1 to 1)
            int totalWords = comment.Split(' ').Length;
            if (totalWords == 0) return 0;
            
            double score = (double)(positiveCount - negativeCount) / totalWords;
            
            // Normalize to range -1 to 1
            return Math.Max(-1, Math.Min(1, score));
        }
    }
}

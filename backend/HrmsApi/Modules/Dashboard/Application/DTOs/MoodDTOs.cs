using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HrmsApi.Modules.Dashboard.Domain;

namespace HrmsApi.Modules.Dashboard.Application.DTOs
{
    public class MoodEntryDTO
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public MoodType Mood { get; set; }
        public string? Comment { get; set; }
        public double? SentimentScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMoodEntryDTO
    {
        [Required]
        public Guid EmployeeId { get; set; }
        
        [Required]
        public MoodType Mood { get; set; }
        
        public string? Comment { get; set; }
    }

    public class MoodAnalyticsDTO
    {
        public double AverageMood { get; set; }
        public int TotalEntries { get; set; }
        public Dictionary<string, int> MoodDistribution { get; set; } = new Dictionary<string, int>();
        public string RecentTrend { get; set; } = string.Empty;
    }
}

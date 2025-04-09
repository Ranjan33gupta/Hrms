using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrmsApi.Modules.Dashboard.Domain
{
    public enum MoodType
    {
        VeryNegative = 0,
        Negative = 1,
        Neutral = 2,
        Positive = 3,
        VeryPositive = 4
    }

    public class MoodEntry
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        public MoodType Mood { get; set; }

        [Column(TypeName = "text")]
        public string? Comment { get; set; }

        public double? SentimentScore { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}

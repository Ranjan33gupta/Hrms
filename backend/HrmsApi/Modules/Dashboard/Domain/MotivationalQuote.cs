using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrmsApi.Modules.Dashboard.Domain
{
    public class MotivationalQuote
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string QuoteText { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Author { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; } // Productivity, Leadership, Teamwork, etc.

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}

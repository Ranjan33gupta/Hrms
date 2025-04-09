using System;
using System.ComponentModel.DataAnnotations;

namespace HrmsApi.Modules.Dashboard.Application.DTOs
{
    public class MotivationalQuoteDTO
    {
        public Guid Id { get; set; }
        public string QuoteText { get; set; } = string.Empty;
        public string? Author { get; set; }
        public string? Category { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateQuoteDTO
    {
        [Required]
        public string QuoteText { get; set; } = string.Empty;
        
        public string? Author { get; set; }
        
        public string? Category { get; set; }
    }
}

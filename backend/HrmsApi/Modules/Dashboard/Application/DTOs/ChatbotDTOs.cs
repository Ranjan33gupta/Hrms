using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HrmsApi.Modules.Dashboard.Application.DTOs
{
    public class ChatbotQueryDTO
    {
        [Required]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public Guid EmployeeId { get; set; }
        
        public string? Language { get; set; } = "en";
    }

    public class ChatbotResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        public string Intent { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public bool RequiresAction { get; set; }
        public string? ApiEndpoint { get; set; }
        public Dictionary<string, string>? Entities { get; set; }
    }

    public class CreateIntentDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public List<string> TrainingPhrases { get; set; } = new List<string>();
        
        public List<EntityDTO> Entities { get; set; } = new List<EntityDTO>();

        [Required]
        public string ResponseTemplate { get; set; } = string.Empty;

        public string? ApiEndpoint { get; set; }
    }

    public class EntityDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Type { get; set; } = string.Empty; // date, number, text, etc.

        public string? Description { get; set; }
    }
}

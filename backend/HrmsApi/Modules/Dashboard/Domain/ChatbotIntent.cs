using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace HrmsApi.Modules.Dashboard.Domain
{
    public class ChatbotIntent
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<ChatbotTrainingPhrase> TrainingPhrases { get; set; } = new List<ChatbotTrainingPhrase>();
        
        public virtual ICollection<ChatbotEntity> Entities { get; set; } = new List<ChatbotEntity>();

        [Required]
        [Column(TypeName = "text")]
        public string ResponseTemplate { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? ApiEndpoint { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ChatbotTrainingPhrase
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid IntentId { get; set; }

        [ForeignKey("IntentId")]
        public virtual ChatbotIntent Intent { get; set; } = null!;

        [Required]
        [Column(TypeName = "text")]
        public string Phrase { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

    public class ChatbotEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid IntentId { get; set; }

        [ForeignKey("IntentId")]
        public virtual ChatbotIntent Intent { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Type { get; set; } = string.Empty; // date, number, text, etc.

        [MaxLength(255)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

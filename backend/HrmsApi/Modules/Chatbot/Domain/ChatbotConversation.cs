using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrmsApi.Modules.Chatbot.Domain
{
    public class ChatbotConversation
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Employee")]
        public Guid? EmployeeId { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }

        public virtual ICollection<ChatbotMessage> Messages { get; set; }
    }

    public class ChatbotMessage
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Conversation")]
        public Guid ConversationId { get; set; }
        public virtual ChatbotConversation Conversation { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }

        [Required]
        public bool IsFromUser { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [ForeignKey("Intent")]
        public Guid? IntentId { get; set; }
        public virtual ChatbotIntent Intent { get; set; }

        public double? ConfidenceScore { get; set; }
    }
}

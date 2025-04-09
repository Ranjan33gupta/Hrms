using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrmsApi.Modules.Chatbot.Domain
{
    public class ChatbotResponse
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Response { get; set; }

        [ForeignKey("Intent")]
        public Guid IntentId { get; set; }
        public virtual ChatbotIntent Intent { get; set; }

        // For randomization of responses
        public int Priority { get; set; } = 0;
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrmsApi.Modules.Chatbot.Domain
{
    public class ChatbotTrainingPhrase
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Phrase { get; set; }

        [ForeignKey("Intent")]
        public Guid IntentId { get; set; }
        public virtual ChatbotIntent Intent { get; set; }
    }
}

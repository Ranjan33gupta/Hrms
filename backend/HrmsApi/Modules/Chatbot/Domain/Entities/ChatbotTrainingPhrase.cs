using System;

namespace HrmsApi.Modules.Chatbot.Domain.Entities
{
    public class ChatbotTrainingPhrase
    {
        public Guid Id { get; set; }
        public Guid IntentId { get; set; }
        public string Phrase { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public virtual ChatbotIntent Intent { get; set; }
    }
}

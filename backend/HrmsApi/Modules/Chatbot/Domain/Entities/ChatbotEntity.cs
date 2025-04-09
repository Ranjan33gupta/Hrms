using System;

namespace HrmsApi.Modules.Chatbot.Domain.Entities
{
    public class ChatbotEntity
    {
        public Guid Id { get; set; }
        public Guid IntentId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public virtual ChatbotIntent Intent { get; set; }
    }
}

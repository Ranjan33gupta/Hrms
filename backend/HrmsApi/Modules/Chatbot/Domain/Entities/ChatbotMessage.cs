using System;

namespace HrmsApi.Modules.Chatbot.Domain.Entities
{
    public class ChatbotMessage
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsFromUser { get; set; }
        
        public virtual ChatbotConversation Conversation { get; set; }
    }
}

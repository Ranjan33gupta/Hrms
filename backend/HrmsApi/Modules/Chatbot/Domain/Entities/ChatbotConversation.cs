using System;
using System.Collections.Generic;

namespace HrmsApi.Modules.Chatbot.Domain.Entities
{
    public class ChatbotConversation
    {
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime LastMessageAt { get; set; }
        public bool IsActive { get; set; }
        
        public virtual ICollection<ChatbotMessage> Messages { get; set; }
        
        public ChatbotConversation()
        {
            Messages = new List<ChatbotMessage>();
        }
    }
}

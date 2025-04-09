using System;
using System.Collections.Generic;

namespace HrmsApi.Modules.Chatbot.Domain.Entities
{
    public class ChatbotIntent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ResponseTemplate { get; set; }
        public string ApiEndpoint { get; set; }
        public string RouteDestination { get; set; }
        public bool RequiresAuth { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        
        public virtual ICollection<ChatbotTrainingPhrase> TrainingPhrases { get; set; }
        public virtual ICollection<ChatbotEntity> Entities { get; set; }
        
        public ChatbotIntent()
        {
            TrainingPhrases = new List<ChatbotTrainingPhrase>();
            Entities = new List<ChatbotEntity>();
        }
    }
}

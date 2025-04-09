using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HrmsApi.Modules.Chatbot.Domain
{
    public class ChatbotIntent
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        public string RequiredRole { get; set; } // Admin, Manager, Employee, or null for all

        public virtual ICollection<ChatbotTrainingPhrase> TrainingPhrases { get; set; }
        public virtual ICollection<ChatbotResponse> Responses { get; set; }

        // Navigation properties
        public string ApiEndpoint { get; set; } // Optional API endpoint to call
        public string RouteDestination { get; set; } // Optional route to navigate to
        public bool RequiresAuth { get; set; } = false;
    }
}

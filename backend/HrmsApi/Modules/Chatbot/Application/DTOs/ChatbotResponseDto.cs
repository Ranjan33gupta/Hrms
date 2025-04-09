using System;
using System.Text.Json.Serialization;

namespace HrmsApi.Modules.Chatbot.Application.DTOs
{
    public class ChatbotResponseDto
    {
        [JsonPropertyName("response")]
        public string Response { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        [JsonPropertyName("conversationId")]
        public Guid? ConversationId { get; set; }
        
        [JsonPropertyName("intent")]
        public string Intent { get; set; }
        
        [JsonPropertyName("intentName")]
        public string IntentName { get; set; }
        
        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
        
        [JsonPropertyName("requiresAuth")]
        public bool RequiresAuth { get; set; }
        
        [JsonPropertyName("routeDestination")]
        public string RouteDestination { get; set; }
        
        [JsonPropertyName("action")]
        public string Action { get; set; }
        
        [JsonPropertyName("apiEndpoint")]
        public string ApiEndpoint { get; set; }
    }
}

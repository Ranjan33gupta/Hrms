using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HrmsApi.Modules.Chatbot.Application.DTOs
{
    public class ChatbotQueryDto
    {
        [JsonPropertyName("query")]
        public string Query { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        [JsonPropertyName("conversationId")]
        public Guid? ConversationId { get; set; }
        
        [JsonPropertyName("employeeId")]
        public Guid? EmployeeId { get; set; }
        
        public bool IsVoiceCommand { get; set; } = false;
        public byte[] VoiceData { get; set; }
    }
}

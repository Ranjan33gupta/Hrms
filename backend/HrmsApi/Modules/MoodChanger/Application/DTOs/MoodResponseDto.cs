using System;
using System.Text.Json.Serialization;

namespace HrmsApi.Modules.MoodChanger.Application.DTOs
{
    public class MoodResponseDto
    {
        [JsonPropertyName("mood")]
        public string Mood { get; set; }
        
        [JsonPropertyName("response")]
        public string Response { get; set; }
        
        [JsonPropertyName("backgroundColor")]
        public string BackgroundColor { get; set; }
        
        [JsonPropertyName("emoji")]
        public string Emoji { get; set; }
        
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
    }
}

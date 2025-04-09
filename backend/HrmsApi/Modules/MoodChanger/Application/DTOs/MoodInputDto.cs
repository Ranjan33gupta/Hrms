using System;
using System.Text.Json.Serialization;

namespace HrmsApi.Modules.MoodChanger.Application.DTOs
{
    public class MoodInputDto
    {
        [JsonPropertyName("input")]
        public string Input { get; set; }
        
        [JsonPropertyName("employeeId")]
        public Guid? EmployeeId { get; set; }
        
        [JsonPropertyName("isAnonymous")]
        public bool IsAnonymous { get; set; } = false;
    }
}

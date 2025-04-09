using System;

namespace HrmsApi.Modules.MoodChanger.Domain.Entities
{
    public class MoodEntry
    {
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public string UserInput { get; set; }
        public string DetectedMood { get; set; }
        public string ResponseContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAnonymous { get; set; }
    }
}

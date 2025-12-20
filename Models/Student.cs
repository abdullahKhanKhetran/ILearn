using System.Text.Json.Serialization;

namespace ILearn.Models
{
    // 🔹 App / API model (NO Supabase inheritance)
    public class Student
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("student_id")]
        public string StudentId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("semester")]
        public int Semester { get; set; }

        [JsonPropertyName("subjects")]
        public Dictionary<string, SubjectMarks>? Subjects { get; set; }

        [JsonPropertyName("attendance")]
        public double Attendance { get; set; }

        [JsonPropertyName("assignments_submitted")]
        public int AssignmentsSubmitted { get; set; }

        [JsonPropertyName("total_assignments")]
        public int TotalAssignments { get; set; }

        [JsonPropertyName("performance_notes")]
        public string PerformanceNotes { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class SubjectMarks
    {
        [JsonPropertyName("marks")]
        public int Marks { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}

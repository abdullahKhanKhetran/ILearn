using System.Text;
using System.Text.Json;
using ILearn.Models;

namespace ILearn.Services
{
    public class SupabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public SupabaseService()
        {
            var url = Environment.GetEnvironmentVariable("Supabase__Url");
            var key = Environment.GetEnvironmentVariable("Supabase__Key");
            
            // Debug logging
            Console.WriteLine($"🔍 Checking Supabase config...");
            Console.WriteLine($"   Supabase__Url: {(string.IsNullOrEmpty(url) ? "NOT FOUND" : url.Substring(0, Math.Min(30, url.Length)) + "...")}");
            Console.WriteLine($"   Supabase__Key: {(string.IsNullOrEmpty(key) ? "NOT FOUND" : "Found (" + key.Length + " chars)")}");
            
            if (string.IsNullOrEmpty(url))
                throw new Exception("❌ Supabase__Url not found in environment variables");
            
            if (string.IsNullOrEmpty(key))
                throw new Exception("❌ Supabase__Key not found in environment variables");

            Console.WriteLine($"✅ Supabase URL loaded: {url[..30]}...");

            _client = new Client(url, key, new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = false
            });
        } 

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            var response = await _httpClient.GetAsync($"{_supabaseUrl}/rest/v1/students?select=*");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Parse as JsonDocument first to handle JSONB properly
            var students = new List<Student>();
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    var student = ParseStudent(element);
                    students.Add(student);
                }
            }

            return students;
        }

        public async Task<Student?> GetStudentByIdAsync(string studentId)
        {
            var response = await _httpClient.GetAsync(
                $"{_supabaseUrl}/rest/v1/students?student_id=eq.{studentId}&select=*"
            );
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("=== RAW JSON FROM SUPABASE ===");
            Console.WriteLine(json);
            Console.WriteLine("================================");

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                var array = doc.RootElement;
                if (array.GetArrayLength() == 0) return null;

                return ParseStudent(array[0]);
            }
        }

        private Student ParseStudent(JsonElement element)
        {
            var student = new Student
            {
                Id = element.GetProperty("id").GetInt64(),
                StudentId = element.GetProperty("student_id").GetString() ?? "",
                Name = element.GetProperty("name").GetString() ?? "",
                Semester = element.GetProperty("semester").GetInt32(),
                Attendance = element.GetProperty("attendance").GetDouble(),
                AssignmentsSubmitted = element.GetProperty("assignments_submitted").GetInt32(),
                TotalAssignments = element.GetProperty("total_assignments").GetInt32(),
                PerformanceNotes = element.GetProperty("performance_notes").GetString() ?? "",
                Subjects = new Dictionary<string, SubjectMarks>()
            };

            // Parse subjects JSONB
            if (element.TryGetProperty("subjects", out JsonElement subjectsElement))
            {
                foreach (var subjectProp in subjectsElement.EnumerateObject())
                {
                    var subjectName = subjectProp.Name;
                    var marks = subjectProp.Value.GetProperty("marks").GetInt32();
                    var total = subjectProp.Value.GetProperty("total").GetInt32();

                    student.Subjects[subjectName] = new SubjectMarks
                    {
                        Marks = marks,
                        Total = total
                    };
                }
            }

            // Parse timestamps if present
            if (element.TryGetProperty("created_at", out JsonElement createdAt))
            {
                student.CreatedAt = createdAt.GetDateTime();
            }
            if (element.TryGetProperty("updated_at", out JsonElement updatedAt))
            {
                student.UpdatedAt = updatedAt.GetDateTime();
            }

            return student;
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            var json = JsonSerializer.Serialize(new
            {
                student_id = student.StudentId,
                name = student.Name,
                semester = student.Semester,
                subjects = student.Subjects,
                attendance = student.Attendance,
                assignments_submitted = student.AssignmentsSubmitted,
                total_assignments = student.TotalAssignments,
                performance_notes = student.PerformanceNotes
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_supabaseUrl}/rest/v1/students", content);
            response.EnsureSuccessStatusCode();

            return student;
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            var json = JsonSerializer.Serialize(new
            {
                name = student.Name,
                semester = student.Semester,
                subjects = student.Subjects,
                attendance = student.Attendance,
                assignments_submitted = student.AssignmentsSubmitted,
                total_assignments = student.TotalAssignments,
                performance_notes = student.PerformanceNotes
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(
                $"{_supabaseUrl}/rest/v1/students?student_id=eq.{student.StudentId}",
                content
            );
            response.EnsureSuccessStatusCode();

            return student;
        }

        public async Task DeleteStudentAsync(string studentId)
        {
            var response = await _httpClient.DeleteAsync(
                $"{_supabaseUrl}/rest/v1/students?student_id=eq.{studentId}"
            );
            response.EnsureSuccessStatusCode();
        }
    }
}
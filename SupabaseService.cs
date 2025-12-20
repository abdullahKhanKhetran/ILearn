using Supabase;
using ILearn.Models;
using Postgrest.Models;
using Postgrest.Attributes;

namespace ILearn.Services
{
    // 🔹 Supabase table mapping (ONLY for DB)
    [Table("students")]
    public class StudentTable : BaseModel
    {
        [PrimaryKey("id")]
        public long Id { get; set; }

        [Column("student_id")]
        public string StudentId { get; set; } = string.Empty;

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("semester")]
        public int Semester { get; set; }

        [Column("subjects")]
        public Dictionary<string, object> Subjects { get; set; } = new();

        [Column("attendance")]
        public double Attendance { get; set; }

        [Column("assignments_submitted")]
        public int AssignmentsSubmitted { get; set; }

        [Column("total_assignments")]
        public int TotalAssignments { get; set; }

        [Column("performance_notes")]
        public string PerformanceNotes { get; set; } = string.Empty;
    }

    // 🔹 Service
    public class SupabaseService
    {
        private readonly Client _client;
        private bool _initialized = false;

        public SupabaseService()
        {
            var url = Environment.GetEnvironmentVariable("Supabase__Url")
                ?? throw new Exception("❌ Supabase__Url not found");
            var key = Environment.GetEnvironmentVariable("Supabase__Key")
                ?? throw new Exception("❌ Supabase__Key not found");

            Console.WriteLine($"✅ Supabase URL loaded: {url[..30]}...");

            _client = new Client(url, key, new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = false
            });
        }

        private async Task InitializeAsync()
        {
            if (_initialized) return;

            Console.WriteLine("⏳ Initializing Supabase...");
            await _client.InitializeAsync();

            // 🔥 HARD PROOF QUERY
            var test = await _client.From<StudentTable>().Limit(1).Get();
            Console.WriteLine($"🔥 Supabase connected. Rows found: {test.Models.Count}");

            _initialized = true;
        }

        // =========================
        // READ
        // =========================
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            await InitializeAsync();

            var response = await _client.From<StudentTable>().Get();
            Console.WriteLine($"✅ Fetched {response.Models.Count} students");

            return response.Models.Select(MapToStudent).ToList();
        }

        public async Task<Student?> GetStudentByIdAsync(string studentId)
        {
            await InitializeAsync();

            var row = await _client
                .From<StudentTable>()
                .Where(x => x.StudentId == studentId)
                .Single();

            return row == null ? null : MapToStudent(row);
        }

        // =========================
        // CREATE
        // =========================
        public async Task<Student> CreateStudentAsync(Student student)
        {
            await InitializeAsync();

            var insert = await _client
                .From<StudentTable>()
                .Insert(MapToTable(student));

            Console.WriteLine($"✅ Student created: {student.StudentId}");
            return MapToStudent(insert.Models.First());
        }

        // =========================
        // UPDATE
        // =========================
        public async Task<Student> UpdateStudentAsync(Student student)
        {
            await InitializeAsync();

            var update = await _client
                .From<StudentTable>()
                .Update(MapToTable(student));

            Console.WriteLine($"✅ Student updated: {student.StudentId}");
            return MapToStudent(update.Models.First());
        }

        // =========================
        // DELETE
        // =========================
        public async Task DeleteStudentAsync(string studentId)
        {
            await InitializeAsync();

            await _client
                .From<StudentTable>()
                .Where(x => x.StudentId == studentId)
                .Delete();

            Console.WriteLine($"🗑️ Student deleted: {studentId}");
        }

        // =========================
        // MAPPERS
        // =========================
        private static Student MapToStudent(StudentTable s) => new()
        {
            Id = s.Id,
            StudentId = s.StudentId,
            Name = s.Name,
            Semester = s.Semester,
            Attendance = s.Attendance,
            AssignmentsSubmitted = s.AssignmentsSubmitted,
            TotalAssignments = s.TotalAssignments,
            PerformanceNotes = s.PerformanceNotes
        };

        private static StudentTable MapToTable(Student s) => new()
        {
            Id = s.Id ?? 0,
            StudentId = s.StudentId,
            Name = s.Name,
            Semester = s.Semester,
            Attendance = s.Attendance,
            AssignmentsSubmitted = s.AssignmentsSubmitted,
            TotalAssignments = s.TotalAssignments,
            PerformanceNotes = s.PerformanceNotes,
            Subjects = s.Subjects?
                .ToDictionary(k => k.Key, v => (object)v.Value)
                ?? new Dictionary<string, object>()
        };
    }
}

using Microsoft.AspNetCore.Mvc;
using ILearn.Models;
using ILearn.Services;
using System.Text.Json;

namespace ILearn.Controllers
{
    public class StudentController : Controller
    {
        private readonly SupabaseService _supabaseService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(SupabaseService supabaseService, ILogger<StudentController> logger)
        {
            _supabaseService = supabaseService;
            _logger = logger;
        }

        // GET: Student
        public async Task<IActionResult> Index()
        {
            try
            {
                var students = await _supabaseService.GetAllStudentsAsync();
                return View(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading students");
                TempData["Error"] = "Failed to load students";
                return View(new List<Student>());
            }
        }

        // GET: Student/Details/S001
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var student = await _supabaseService.GetStudentByIdAsync(id);
                if (student == null) return NotFound();
                return View(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading student details");
                return NotFound();
            }
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, string subjectsJson)
        {
            try
            {
                // Parse subjects JSON if provided
                if (!string.IsNullOrEmpty(subjectsJson))
                {
                    var subjectsDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, SubjectMarks>>(subjectsJson);
                    if (subjectsDict != null)
                    {
                        student.Subjects = subjectsDict;
                    }
                }
                
                // Validate required fields
                if (string.IsNullOrEmpty(student.StudentId) || string.IsNullOrEmpty(student.Name))
                {
                    TempData["Error"] = "Student ID and Name are required";
                    return View(student);
                }

                await _supabaseService.CreateStudentAsync(student);
                TempData["Success"] = "Student created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student");
                TempData["Error"] = $"Failed to create student: {ex.Message}";
                return View(student);
            }
        }

        // GET: Student/Edit/S001
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var student = await _supabaseService.GetStudentByIdAsync(id);
                if (student == null) return NotFound();
                return View(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading student for edit");
                return NotFound();
            }
        }

        // POST: Student/Edit/S001
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Student student, string subjectsJson)
        {
            if (id != student.StudentId) return NotFound();

            try
            {
                // Parse subjects JSON if provided
                if (!string.IsNullOrEmpty(subjectsJson))
                {
                    var subjectsDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, SubjectMarks>>(subjectsJson);
                    if (subjectsDict != null)
                    {
                        student.Subjects = subjectsDict;
                    }
                }
                
                // Ensure StudentId matches
                student.StudentId = id;
                
                // Validate required fields
                if (string.IsNullOrEmpty(student.Name))
                {
                    TempData["Error"] = "Name is required";
                    return View(student);
                }

                await _supabaseService.UpdateStudentAsync(student);
                TempData["Success"] = "Student updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student");
                TempData["Error"] = $"Failed to update student: {ex.Message}";
                return View(student);
            }
        }

        // GET: Student/Delete/S001
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var student = await _supabaseService.GetStudentByIdAsync(id);
                if (student == null) return NotFound();
                return View(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading student for delete");
                return NotFound();
            }
        }

        // POST: Student/Delete/S001
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _supabaseService.DeleteStudentAsync(id);
                TempData["Success"] = "Student deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student");
                TempData["Error"] = "Failed to delete student";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
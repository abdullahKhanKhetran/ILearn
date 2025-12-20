using Microsoft.AspNetCore.Mvc;
using ILearn.Models;
using ILearn.Services;

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
        public async Task<IActionResult> Create(Student student)
        {
            try
            {
                await _supabaseService.CreateStudentAsync(student);
                TempData["Success"] = "Student created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student");
                TempData["Error"] = "Failed to create student";
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
        public async Task<IActionResult> Edit(string id, Student student)
        {
            if (id != student.StudentId) return NotFound();

            try
            {
                await _supabaseService.UpdateStudentAsync(student);
                TempData["Success"] = "Student updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student");
                TempData["Error"] = "Failed to update student";
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
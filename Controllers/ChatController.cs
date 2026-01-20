using Microsoft.AspNetCore.Mvc;
using ILearn.Models;
using ILearn.Services;

namespace ILearn.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;
        private readonly SupabaseService _supabaseService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            ChatService chatService,
            SupabaseService supabaseService,
            ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _supabaseService = supabaseService;
            _logger = logger;
        }

        // GET: Chat
        public async Task<IActionResult> Index()
        {
            ViewBag.ApiStatus = false;
            ViewBag.Students = new List<Student>();

            try
            {
                // Check if Python API is running
                ViewBag.ApiStatus = await _chatService.CheckHealthAsync();

                // Load students for dropdown
                ViewBag.Students = await _supabaseService.GetAllStudentsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading chat page");
            }
            
            return View();
        }

        // POST: Chat/SendMessage (AJAX endpoint)
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            try
            {
                var response = await _chatService.SendMessageAsync(request);
                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                return StatusCode(500, new { error = "Failed to send message", details = ex.Message });
            }
        }

        // GET: Chat/Health (Check API status)
        [HttpGet]
        public async Task<IActionResult> Health()
        {
            try
            {
                var isHealthy = await _chatService.CheckHealthAsync();
                return Json(new { status = isHealthy ? "healthy" : "unhealthy" });
            }
            catch
            {
                return Json(new { status = "unhealthy" });
            }
        }
    }
}
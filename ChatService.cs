using System.Text;
using System.Text.Json;
using ILearn.Models;

namespace ILearn.Services
{
    public class ChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public ChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ChatbotApi:BaseUrl"] ?? "http://localhost:8000";
        }

        public async Task<ChatResponse> SendMessageAsync(ChatRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/chat", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ChatResponse>(responseJson)
                ?? throw new Exception("Failed to deserialize response");
        }

        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
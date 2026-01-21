using System.Text;
using System.Text.Json;
using ILearn.Models;

namespace ILearn.Services
{
    public class ChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["PythonApi:BaseUrl"] ?? "http://localhost:8000";
            
            // Configure JSON options to respect JsonPropertyName attributes
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ChatResponse> SendMessageAsync(ChatRequest request)
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/chat", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ChatResponse>(responseJson, _jsonOptions)
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
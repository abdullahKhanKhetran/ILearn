using System.Text.Json.Serialization;

namespace ILearn.Models
{
    public class ChatRequest
    {
        [JsonPropertyName("student_id")]
        public string StudentId { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("conversation_history")]
        public List<ChatMessage> ConversationHistory { get; set; } = new();
    }

    public class ChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    public class ChatResponse
    {
        [JsonPropertyName("student_id")]
        public string StudentId { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string UserMessage { get; set; } = string.Empty;

        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        [JsonPropertyName("performance_category")]
        public string? PerformanceCategory { get; set; }

        [JsonPropertyName("conversation_history")]
        public List<ChatMessage> ConversationHistory { get; set; } = new();

        [JsonPropertyName("suggestions")]
        public List<string> Suggestions { get; set; } = new();
    }
}

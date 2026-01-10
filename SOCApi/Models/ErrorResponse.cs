using System.Text.Json.Serialization;

namespace SOCApi.Models
{
    /// <summary>
    /// Standardized error response model for API consumers.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// HTTP status code.
        /// </summary>
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        /// <summary>
        /// Error message for the user.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Detailed error information (only in development).
        /// </summary>
        [JsonPropertyName("details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Details { get; set; }

        /// <summary>
        /// Unique trace identifier for tracking the error.
        /// </summary>
        [JsonPropertyName("traceId")]
        public string TraceId { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the error occurred.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Path where the error occurred.
        /// </summary>
        [JsonPropertyName("path")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Path { get; set; }

        /// <summary>
        /// Validation errors (for 400 Bad Request).
        /// </summary>
        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace MultiFamilyPortal.CoreUI.Models
{
    internal class CaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("challenge_ts")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("error-codes")]
        public IEnumerable<string> ErrorCodes { get; set; }
    }
}

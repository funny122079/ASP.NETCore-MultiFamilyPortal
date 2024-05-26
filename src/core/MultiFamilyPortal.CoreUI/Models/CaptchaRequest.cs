using System.Text.Json.Serialization;

namespace MultiFamilyPortal.CoreUI.Models
{
    internal class CaptchaRequest
    {
        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        [JsonPropertyName("response")]
        public string Response { get; set; }
    }
}

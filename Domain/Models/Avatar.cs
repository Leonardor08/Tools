using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Avatar
    {
        [JsonPropertyName("href")]
        public string Href { get; set; } = string.Empty;
    }
}
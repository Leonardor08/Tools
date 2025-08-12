using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Links
    {
        [JsonPropertyName("avatar")]
        public Avatar Avatar { get; set; } = new();
    }
}
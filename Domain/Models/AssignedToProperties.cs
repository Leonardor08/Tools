using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class AssignedToProperties
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        [JsonPropertyName("_links")]
        public Links Links { get; set; } = new();
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("uniqueName")]
        public string UniqueName { get; set; } = string.Empty;
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;
        [JsonPropertyName("descriptor")]
        public string Descriptor { get; set; } = string.Empty;

        [JsonPropertyName("inactive")]
        public bool? Inactive { get; set; }
    }
}
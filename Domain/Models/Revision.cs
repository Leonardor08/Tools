using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Revision
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("value")]
        public List<RevisionProperties> Value { get; set; } = [];
    }
}

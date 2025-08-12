using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class CommentVersionRef
    {
        [JsonPropertyName("commentId")]
        public int CommentId { get; set; }
        [JsonPropertyName("version")]
        public int Version { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}
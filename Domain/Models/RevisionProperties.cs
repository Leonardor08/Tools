using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class RevisionProperties
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("rev")]
        public int Rev { get; set; }

        [JsonPropertyName("fields")]
        public AzureFields Fields { get; set; } = new();

        [JsonPropertyName("multilineFieldsFormat")]
        public MultiLineFields MultilineFieldsFormat { get; set; } = new();

        [JsonPropertyName("commentVersionRef")]
        public CommentVersionRef CommentVersionRef { get; set; } = new();

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}
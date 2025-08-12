using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class MultiLineFields
    {
        [JsonPropertyName("System.Description")]
        public string SystemDescription { get; set; } = string.Empty;
        [JsonPropertyName("System.History")]
        public string SystemHistory { get; set; } = string.Empty;
        [JsonPropertyName("Microsoft.VSTS.Common.AcceptanceCriteria")]
        public string AcceptanceCriteria { get; set; } = string.Empty;
    }
}
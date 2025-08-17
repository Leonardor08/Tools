using System.Text.Json.Serialization;

namespace Domain.Models;
public class Columns
{
    [JsonPropertyName("referenceName")]
    public string ReferenceName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

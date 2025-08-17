using System.Text.Json.Serialization;

namespace Domain.Models;

public class Field
{
    [JsonPropertyName("ReferenceName")]
    public string ReferenceName { get; set; } = string.Empty;
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("Url")]
    public string Url { get; set; } = string.Empty;
}

using System.Text.Json.Serialization;

namespace Domain.Models;

public class WorkItems
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

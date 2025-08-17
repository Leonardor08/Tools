using System.Text.Json.Serialization;

namespace Domain.Models;

public class SortColumns
{
    [JsonPropertyName("field")]
    public Field Field { get; set; } = new();
    [JsonPropertyName("descending")]
    public bool Descending { get; set; } = new();
}

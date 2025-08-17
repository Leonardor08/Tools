using System.Text.Json.Serialization;

namespace Domain.Models;

public class Query
{
    [JsonPropertyName("queryType")]
    public string QueryType { get; set; } = string.Empty;

    [JsonPropertyName("queryResultType")]
    public string QueryResultType { get; set; } = string.Empty;

    [JsonPropertyName("asOf")]
    public DateTime AsOf { get; set; }

    [JsonPropertyName("columns")]
    public List<Columns> Columns { get; set; } = [];

    [JsonPropertyName("sortColumns")]
    public List<SortColumns>? SortColumns { get; set; } = [];

    [JsonPropertyName("workItems")]
    public List<WorkItems> WorkItems { get; set; } = [];
}

using System.Text.Json.Serialization;

namespace Infraestructure.Adaptadores.AzureDevOps.Contratos;

// Contratos JSON de la API REST de Azure DevOps (solo los campos que usa el sistema).
// Viven en infraestructura: si Microsoft cambia el formato, el dominio no se entera.

internal sealed class RespuestaRevisiones
{
    [JsonPropertyName("count")] public int Count { get; set; }
    [JsonPropertyName("value")] public List<RevisionApi> Value { get; set; } = [];
}

internal sealed class RevisionApi
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("rev")] public int Rev { get; set; }
    [JsonPropertyName("fields")] public CamposApi Fields { get; set; } = new();
}

internal sealed class CamposApi
{
    [JsonPropertyName("System.AssignedTo")] public IdentidadApi? AssignedTo { get; set; }
    [JsonPropertyName("System.ChangedBy")] public IdentidadApi? ChangedBy { get; set; }
    [JsonPropertyName("System.ChangedDate")] public DateTime ChangedDate { get; set; }
    [JsonPropertyName("System.History")] public string? History { get; set; }
    [JsonPropertyName("Microsoft.VSTS.Scheduling.DueDate")] public DateTime? DueDate { get; set; }
}

internal sealed class IdentidadApi
{
    [JsonPropertyName("displayName")] public string DisplayName { get; set; } = string.Empty;
    [JsonPropertyName("uniqueName")] public string UniqueName { get; set; } = string.Empty;
}

internal sealed class RespuestaWiql
{
    [JsonPropertyName("asOf")] public DateTime AsOf { get; set; }
    [JsonPropertyName("workItems")] public List<ReferenciaWorkItemApi> WorkItems { get; set; } = [];
}

internal sealed class ReferenciaWorkItemApi
{
    [JsonPropertyName("id")] public int Id { get; set; }
}

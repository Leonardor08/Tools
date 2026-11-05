namespace Domain.Consultas;

/// <summary>Resultado de una consulta de work items: qué IDs devolvió y a qué fecha de corte.</summary>
public sealed record ResultadoConsulta(DateTime FechaCorte, IReadOnlyList<int> WorkItemIds)
{
    public int Total => WorkItemIds.Count;
}

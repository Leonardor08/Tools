using Domain.Trazabilidad;

namespace Application.Puertos.Salida;

/// <summary>Puerto de salida: obtener el historial de revisiones de un work item.</summary>
public interface IRevisionesWorkItemPuerto
{
    Task<IReadOnlyList<RevisionWorkItem>> ObtenerRevisionesAsync(
        string organizacion, string proyecto, int workItemId, CancellationToken ct);
}

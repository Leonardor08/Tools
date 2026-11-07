using Domain.Auditoria;

namespace Application.Puertos.Salida;

/// <summary>Puerto de salida del lado de escritura: persistencia de cortes de trazabilidad.</summary>
public interface ICortesTrazabilidadRepositorio
{
    Task GuardarAsync(CorteTrazabilidad corte, CancellationToken ct);
    Task<CorteTrazabilidad?> ObtenerAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<CorteTrazabilidad>> ListarPorWorkItemAsync(string organizacion, string proyecto, int workItemId, CancellationToken ct);
}

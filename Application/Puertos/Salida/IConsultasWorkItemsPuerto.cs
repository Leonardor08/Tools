using Domain.Consultas;

namespace Application.Puertos.Salida;

/// <summary>Puerto de salida: ejecutar consultas de work items (consultas guardadas o WIQL).</summary>
public interface IConsultasWorkItemsPuerto
{
    Task<ResultadoConsulta> EjecutarConsultaGuardadaAsync(
        string organizacion, string proyecto, Guid consultaId, CancellationToken ct);

    Task<ResultadoConsulta> EjecutarWiqlAsync(
        string organizacion, string proyecto, string wiql, CancellationToken ct);
}

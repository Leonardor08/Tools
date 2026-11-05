using Application.Dtos;

namespace Application.Puertos.Entrada;

// Puertos de ENTRADA: lo que el mundo exterior (API, CLI, pruebas) puede pedirle a la aplicación.

public interface IObtenerTrazabilidadWorkItem
{
    Task<TrazabilidadWorkItemDto> EjecutarAsync(string organizacion, string proyecto, int workItemId, CancellationToken ct);
}

public interface IEjecutarConsultaGuardada
{
    Task<ResultadoConsultaDto> EjecutarAsync(string organizacion, string proyecto, Guid consultaId, CancellationToken ct);
}

public interface IConsultarHistoriasPorResponsable
{
    Task<ResultadoConsultaDto> EjecutarAsync(string organizacion, string proyecto, string responsable, CancellationToken ct);
}

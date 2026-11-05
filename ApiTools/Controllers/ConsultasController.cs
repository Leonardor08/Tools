using Application.Dtos;
using Application.Puertos.Entrada;
using Microsoft.AspNetCore.Mvc;

namespace ApiTools.Controllers;

[ApiController]
[Route("api/{organizacion}/{proyecto}/consultas")]
public sealed class ConsultasController(
    IEjecutarConsultaGuardada consultaGuardada,
    IConsultarHistoriasPorResponsable historiasPorResponsable) : ControllerBase
{
    /// <summary>Ejecuta una consulta guardada de Azure DevOps.</summary>
    [HttpGet("{consultaId:guid}")]
    [ProducesResponseType<ResultadoConsultaDto>(StatusCodes.Status200OK)]
    public Task<ResultadoConsultaDto> EjecutarAsync(string organizacion, string proyecto, Guid consultaId, CancellationToken ct)
        => consultaGuardada.EjecutarAsync(organizacion, proyecto, consultaId, ct);

    /// <summary>Historias de usuario activas de un responsable (nombre o correo, como aparece en Azure DevOps).</summary>
    [HttpGet("historias")]
    [ProducesResponseType<ResultadoConsultaDto>(StatusCodes.Status200OK)]
    public Task<ResultadoConsultaDto> HistoriasAsync(string organizacion, string proyecto, [FromQuery] string responsable, CancellationToken ct)
        => historiasPorResponsable.EjecutarAsync(organizacion, proyecto, responsable, ct);
}

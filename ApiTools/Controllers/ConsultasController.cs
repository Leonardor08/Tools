using Application.Consultas;
using Application.Cqrs;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ApiTools.Controllers;

[ApiController]
[Route("api/{organizacion}/{proyecto}/consultas")]
public sealed class ConsultasController(IMediador mediador) : ControllerBase
{
    /// <summary>Ejecuta una consulta guardada de Azure DevOps.</summary>
    [HttpGet("{consultaId:guid}")]
    [ProducesResponseType<ResultadoConsultaDto>(StatusCodes.Status200OK)]
    public Task<ResultadoConsultaDto> EjecutarAsync(string organizacion, string proyecto, Guid consultaId, CancellationToken ct)
        => mediador.EnviarAsync(new EjecutarConsultaGuardadaConsulta(organizacion, proyecto, consultaId), ct);

    /// <summary>Historias de usuario activas de un responsable (nombre o correo, como aparece en Azure DevOps).</summary>
    [HttpGet("historias")]
    [ProducesResponseType<ResultadoConsultaDto>(StatusCodes.Status200OK)]
    public Task<ResultadoConsultaDto> HistoriasAsync(string organizacion, string proyecto, [FromQuery] string responsable, CancellationToken ct)
        => mediador.EnviarAsync(new HistoriasPorResponsableConsulta(organizacion, proyecto, responsable), ct);
}

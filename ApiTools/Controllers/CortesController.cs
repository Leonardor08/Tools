using Application.Comandos;
using Application.Consultas;
using Application.Cqrs;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ApiTools.Controllers;

/// <summary>Cortes de trazabilidad: el POST es un COMANDO (escritura); los GET son CONSULTAS (lectura).</summary>
[ApiController]
public sealed class CortesController(IMediador mediador) : ControllerBase
{
    public sealed record SolicitudCorte(string Motivo);

    /// <summary>Congela la trazabilidad actual del work item como evidencia de auditoría.</summary>
    [HttpPost("api/{organizacion}/{proyecto}/workitems/{workItemId:int}/cortes")]
    [ProducesResponseType<CorteRegistradoDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RegistrarAsync(string organizacion, string proyecto, int workItemId,
        [FromBody] SolicitudCorte solicitud, CancellationToken ct)
    {
        var corte = await mediador.EnviarAsync(new RegistrarCorteComando(organizacion, proyecto, workItemId, solicitud.Motivo), ct);
        return CreatedAtAction(nameof(ObtenerAsync), new { corteId = corte.Id }, corte);
    }

    /// <summary>Cortes registrados de un work item (más reciente primero).</summary>
    [HttpGet("api/{organizacion}/{proyecto}/workitems/{workItemId:int}/cortes")]
    [ProducesResponseType<IReadOnlyList<CorteResumenDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<CorteResumenDto>> ListarAsync(string organizacion, string proyecto, int workItemId, CancellationToken ct)
        => mediador.EnviarAsync(new ListarCortesConsulta(organizacion, proyecto, workItemId), ct);

    /// <summary>Detalle de un corte con verificación de integridad (huella SHA-256).</summary>
    [HttpGet("api/cortes/{corteId:guid}")]
    [ProducesResponseType<CorteDetalleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<CorteDetalleDto> ObtenerAsync(Guid corteId, CancellationToken ct)
        => mediador.EnviarAsync(new ObtenerCorteConsulta(corteId), ct);
}

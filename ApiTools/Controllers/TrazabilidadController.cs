using Application.Consultas;
using Application.Cqrs;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ApiTools.Controllers;

/// <summary>Adaptador de entrada HTTP: traduce la petición a una CONSULTA y la envía al mediador.</summary>
[ApiController]
[Route("api/{organizacion}/{proyecto}/workitems")]
public sealed class TrazabilidadController(IMediador mediador) : ControllerBase
{
    /// <summary>Periodos de asignación de un work item: responsable, duración, vencimiento y comentarios.</summary>
    [HttpGet("{workItemId:int}/trazabilidad")]
    [ProducesResponseType<TrazabilidadWorkItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public Task<TrazabilidadWorkItemDto> ObtenerAsync(string organizacion, string proyecto, int workItemId, CancellationToken ct)
        => mediador.EnviarAsync(new ObtenerTrazabilidadConsulta(organizacion, proyecto, workItemId), ct);
}

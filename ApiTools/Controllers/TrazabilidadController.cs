using Application.Dtos;
using Application.Puertos.Entrada;
using Microsoft.AspNetCore.Mvc;

namespace ApiTools.Controllers;

/// <summary>Adaptador de entrada HTTP: el controlador solo traduce HTTP ⇄ caso de uso.</summary>
[ApiController]
[Route("api/{organizacion}/{proyecto}/workitems")]
public sealed class TrazabilidadController(IObtenerTrazabilidadWorkItem obtenerTrazabilidad) : ControllerBase
{
    /// <summary>Periodos de asignación de un work item: responsable, duración, vencimiento y comentarios.</summary>
    [HttpGet("{workItemId:int}/trazabilidad")]
    [ProducesResponseType<TrazabilidadWorkItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public Task<TrazabilidadWorkItemDto> ObtenerAsync(string organizacion, string proyecto, int workItemId, CancellationToken ct)
        => obtenerTrazabilidad.EjecutarAsync(organizacion, proyecto, workItemId, ct);
}

using Application.Dtos;
using Application.Excepciones;
using Application.Puertos.Entrada;
using Application.Puertos.Salida;
using Domain.Trazabilidad;

namespace Application.CasosDeUso;

/// <summary>Caso de uso: ¿quién tuvo el work item, durante cuánto tiempo y qué comentó?</summary>
internal sealed class ObtenerTrazabilidadWorkItem(
    IRevisionesWorkItemPuerto revisiones, ILimpiadorHtml limpiador, IReloj reloj) : IObtenerTrazabilidadWorkItem
{
    public async Task<TrazabilidadWorkItemDto> EjecutarAsync(string organizacion, string proyecto, int workItemId, CancellationToken ct)
    {
        Validar.OrganizacionYProyecto(organizacion, proyecto);
        if (workItemId <= 0)
            throw ValidacionException.De("workItemId", "El identificador del work item debe ser positivo.");

        var historial = await revisiones.ObtenerRevisionesAsync(organizacion, proyecto, workItemId, ct);
        var periodos = CalculadoraTrazabilidad.Calcular(historial);
        var ahora = reloj.AhoraUtc;

        return new TrazabilidadWorkItemDto(
            workItemId,
            historial.Count,
            periodos.Select(p => new PeriodoAsignacionDto(
                p.Responsable, p.Desde, p.Hasta, p.Vigente,
                Math.Round(p.Duracion(ahora).TotalHours, 1),
                p.FechaLimite, p.Vencido(ahora),
                p.Comentarios.Select(limpiador.ATextoPlano).ToList())).ToList());
    }
}

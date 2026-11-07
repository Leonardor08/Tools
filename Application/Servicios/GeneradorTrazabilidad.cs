using Application.Dtos;
using Application.Puertos.Salida;
using Domain.Trazabilidad;

namespace Application.Servicios;

/// <summary>Lógica compartida por la consulta de trazabilidad y el comando de corte.</summary>
internal sealed class GeneradorTrazabilidad(IRevisionesWorkItemPuerto revisiones, ILimpiadorHtml limpiador)
{
    public async Task<(int TotalRevisiones, IReadOnlyList<PeriodoAsignacion> Periodos)> GenerarAsync(
        string organizacion, string proyecto, int workItemId, CancellationToken ct)
    {
        var historial = await revisiones.ObtenerRevisionesAsync(organizacion, proyecto, workItemId, ct);
        var periodos = CalculadoraTrazabilidad.Calcular(historial)
            .Select(p => p with { Comentarios = p.Comentarios.Select(limpiador.ATextoPlano).ToList() })
            .ToList();
        return (historial.Count, periodos);
    }

    public static PeriodoAsignacionDto ADto(PeriodoAsignacion p, DateTime ahora) => new(
        p.Responsable, p.Desde, p.Hasta, p.Vigente, Math.Round(p.Duracion(ahora).TotalHours, 1),
        p.FechaLimite, p.Vencido(ahora), p.Comentarios);
}

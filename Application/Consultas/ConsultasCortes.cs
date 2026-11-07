using Application.Cqrs;
using Application.Dtos;
using Application.Excepciones;
using Application.Puertos.Salida;
using Application.Servicios;
using Domain.Auditoria;

namespace Application.Consultas;

// LADO DE LECTURA de los cortes: proyecciones pensadas para quien consulta (no para quien escribe).
// No son cacheables: deben reflejar de inmediato un corte recién registrado.

/// <summary>CONSULTA: cortes registrados de un work item (resumen, sin el detalle de periodos).</summary>
public sealed record ListarCortesConsulta(string Organizacion, string Proyecto, int WorkItemId)
    : IConsulta<IReadOnlyList<CorteResumenDto>>;

internal sealed class ListarCortesValidador : IValidador<ListarCortesConsulta>
{
    public IEnumerable<(string, string)> Validar(ListarCortesConsulta c) =>
        Reglas.OrganizacionYProyecto(c.Organizacion, c.Proyecto).Concat(Reglas.WorkItem(c.WorkItemId));
}

internal sealed class ListarCortesManejador(ICortesTrazabilidadRepositorio cortes)
    : IManejador<ListarCortesConsulta, IReadOnlyList<CorteResumenDto>>
{
    public async Task<IReadOnlyList<CorteResumenDto>> ManejarAsync(ListarCortesConsulta c, CancellationToken ct)
    {
        var lista = await cortes.ListarPorWorkItemAsync(c.Organizacion, c.Proyecto, c.WorkItemId, ct);
        return lista.OrderByDescending(x => x.FechaCorteUtc)
            .Select(x => new CorteResumenDto(x.Id, x.WorkItemId, x.FechaCorteUtc, x.Motivo, x.Periodos.Count, x.Huella, x.EsIntegro()))
            .ToList();
    }
}

/// <summary>CONSULTA: detalle completo de un corte, con verificación de integridad.</summary>
public sealed record ObtenerCorteConsulta(Guid CorteId) : IConsulta<CorteDetalleDto>;

internal sealed class ObtenerCorteManejador(ICortesTrazabilidadRepositorio cortes)
    : IManejador<ObtenerCorteConsulta, CorteDetalleDto>
{
    public async Task<CorteDetalleDto> ManejarAsync(ObtenerCorteConsulta c, CancellationToken ct)
    {
        var corte = await cortes.ObtenerAsync(c.CorteId, ct)
                    ?? throw new RecursoNoEncontradoException($"El corte {c.CorteId} no existe.");
        return ADetalle(corte);
    }

    internal static CorteDetalleDto ADetalle(CorteTrazabilidad c) => new(
        c.Id, c.Organizacion, c.Proyecto, c.WorkItemId, c.FechaCorteUtc, c.Motivo, c.Huella, c.EsIntegro(),
        // En un corte, las horas se miden contra la fecha del corte, no contra "ahora".
        c.Periodos.Select(p => GeneradorTrazabilidad.ADto(p, c.FechaCorteUtc)).ToList());
}

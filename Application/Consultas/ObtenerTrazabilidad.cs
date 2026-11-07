using Application.Cqrs;
using Application.Dtos;
using Application.Puertos.Salida;
using Application.Servicios;

namespace Application.Consultas;

/// <summary>CONSULTA: ¿quién tuvo el work item, cuánto tiempo y qué comentó? (cacheable: lee Azure DevOps)</summary>
public sealed record ObtenerTrazabilidadConsulta(string Organizacion, string Proyecto, int WorkItemId)
    : IConsulta<TrazabilidadWorkItemDto>, IConsultaCacheable
{
    public string ClaveCache => $"{Organizacion}/{Proyecto}/{WorkItemId}".ToLowerInvariant();
    public TimeSpan DuracionCache => TimeSpan.FromMinutes(2);
}

internal sealed class ObtenerTrazabilidadValidador : IValidador<ObtenerTrazabilidadConsulta>
{
    public IEnumerable<(string, string)> Validar(ObtenerTrazabilidadConsulta c) =>
        Reglas.OrganizacionYProyecto(c.Organizacion, c.Proyecto).Concat(Reglas.WorkItem(c.WorkItemId));
}

internal sealed class ObtenerTrazabilidadManejador(GeneradorTrazabilidad generador, IReloj reloj)
    : IManejador<ObtenerTrazabilidadConsulta, TrazabilidadWorkItemDto>
{
    public async Task<TrazabilidadWorkItemDto> ManejarAsync(ObtenerTrazabilidadConsulta c, CancellationToken ct)
    {
        var (total, periodos) = await generador.GenerarAsync(c.Organizacion, c.Proyecto, c.WorkItemId, ct);
        var ahora = reloj.AhoraUtc;
        return new TrazabilidadWorkItemDto(c.WorkItemId, total, periodos.Select(p => GeneradorTrazabilidad.ADto(p, ahora)).ToList());
    }
}

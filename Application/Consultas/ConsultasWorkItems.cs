using Application.Cqrs;
using Application.Dtos;
using Application.Puertos.Salida;
using Domain.Consultas;

namespace Application.Consultas;

/// <summary>CONSULTA: ejecutar una consulta guardada de Azure DevOps.</summary>
public sealed record EjecutarConsultaGuardadaConsulta(string Organizacion, string Proyecto, Guid ConsultaId)
    : IConsulta<ResultadoConsultaDto>, IConsultaCacheable
{
    public string ClaveCache => $"{Organizacion}/{Proyecto}/{ConsultaId}".ToLowerInvariant();
    public TimeSpan DuracionCache => TimeSpan.FromMinutes(1);
}

internal sealed class EjecutarConsultaGuardadaValidador : IValidador<EjecutarConsultaGuardadaConsulta>
{
    public IEnumerable<(string, string)> Validar(EjecutarConsultaGuardadaConsulta c)
    {
        foreach (var e in Reglas.OrganizacionYProyecto(c.Organizacion, c.Proyecto)) yield return e;
        if (c.ConsultaId == Guid.Empty) yield return ("consultaId", "El identificador de la consulta es obligatorio.");
    }
}

internal sealed class EjecutarConsultaGuardadaManejador(IConsultasWorkItemsPuerto consultas)
    : IManejador<EjecutarConsultaGuardadaConsulta, ResultadoConsultaDto>
{
    public async Task<ResultadoConsultaDto> ManejarAsync(EjecutarConsultaGuardadaConsulta c, CancellationToken ct)
        => Mapear(await consultas.EjecutarConsultaGuardadaAsync(c.Organizacion, c.Proyecto, c.ConsultaId, ct));

    internal static ResultadoConsultaDto Mapear(ResultadoConsulta r) => new(r.FechaCorte, r.Total, r.WorkItemIds);
}

/// <summary>CONSULTA: historias de usuario activas asignadas a un responsable.</summary>
public sealed record HistoriasPorResponsableConsulta(string Organizacion, string Proyecto, string Responsable)
    : IConsulta<ResultadoConsultaDto>, IConsultaCacheable
{
    public string ClaveCache => $"{Organizacion}/{Proyecto}/{Responsable}".ToLowerInvariant();
    public TimeSpan DuracionCache => TimeSpan.FromMinutes(1);
}

internal sealed class HistoriasPorResponsableValidador : IValidador<HistoriasPorResponsableConsulta>
{
    public IEnumerable<(string, string)> Validar(HistoriasPorResponsableConsulta c)
    {
        foreach (var e in Reglas.OrganizacionYProyecto(c.Organizacion, c.Proyecto)) yield return e;
        if (string.IsNullOrWhiteSpace(c.Responsable) || c.Responsable.Length > 120)
            yield return ("responsable", "Indique el nombre o correo del responsable (máximo 120 caracteres).");
    }
}

internal sealed class HistoriasPorResponsableManejador(IConsultasWorkItemsPuerto consultas)
    : IManejador<HistoriasPorResponsableConsulta, ResultadoConsultaDto>
{
    public async Task<ResultadoConsultaDto> ManejarAsync(HistoriasPorResponsableConsulta c, CancellationToken ct)
    {
        string wiql = ConstructorWiql.HistoriasActivasDe(c.Proyecto, c.Responsable);
        return EjecutarConsultaGuardadaManejador.Mapear(await consultas.EjecutarWiqlAsync(c.Organizacion, c.Proyecto, wiql, ct));
    }
}

/// <summary>Construye WIQL escapando los valores para evitar inyección (las comillas simples se duplican).</summary>
internal static class ConstructorWiql
{
    public static string HistoriasActivasDe(string proyecto, string responsable) =>
        $"""
        SELECT [System.Id], [System.Title], [System.AssignedTo], [System.State]
        FROM workitems
        WHERE [System.TeamProject] = '{Escapar(proyecto)}'
          AND [System.WorkItemType] = 'User Story'
          AND [System.State] <> 'Removed'
          AND [System.AssignedTo] = '{Escapar(responsable)}'
        ORDER BY [System.ChangedDate] DESC
        """;

    internal static string Escapar(string valor) => valor.Trim().Replace("'", "''");
}

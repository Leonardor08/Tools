using Application.Dtos;
using Application.Excepciones;
using Application.Puertos.Entrada;
using Application.Puertos.Salida;
using Domain.Consultas;

namespace Application.CasosDeUso;

/// <summary>Caso de uso: ejecutar una consulta guardada de Azure DevOps por su identificador.</summary>
internal sealed class EjecutarConsultaGuardada(IConsultasWorkItemsPuerto consultas) : IEjecutarConsultaGuardada
{
    public async Task<ResultadoConsultaDto> EjecutarAsync(string organizacion, string proyecto, Guid consultaId, CancellationToken ct)
    {
        Validar.OrganizacionYProyecto(organizacion, proyecto);
        if (consultaId == Guid.Empty)
            throw ValidacionException.De("consultaId", "El identificador de la consulta es obligatorio.");

        return Mapear(await consultas.EjecutarConsultaGuardadaAsync(organizacion, proyecto, consultaId, ct));
    }

    internal static ResultadoConsultaDto Mapear(ResultadoConsulta r) => new(r.FechaCorte, r.Total, r.WorkItemIds);
}

/// <summary>Caso de uso: historias de usuario activas asignadas a un responsable.</summary>
internal sealed class ConsultarHistoriasPorResponsable(IConsultasWorkItemsPuerto consultas) : IConsultarHistoriasPorResponsable
{
    public async Task<ResultadoConsultaDto> EjecutarAsync(string organizacion, string proyecto, string responsable, CancellationToken ct)
    {
        Validar.OrganizacionYProyecto(organizacion, proyecto);
        if (string.IsNullOrWhiteSpace(responsable) || responsable.Length > 120)
            throw ValidacionException.De("responsable", "Indique el nombre o correo del responsable (máximo 120 caracteres).");

        string wiql = ConstructorWiql.HistoriasActivasDe(proyecto, responsable);
        return EjecutarConsultaGuardada.Mapear(await consultas.EjecutarWiqlAsync(organizacion, proyecto, wiql, ct));
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

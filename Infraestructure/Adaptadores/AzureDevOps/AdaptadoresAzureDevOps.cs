using Application.Puertos.Salida;
using Domain.Consultas;
using Domain.Trazabilidad;
using Infraestructure.Adaptadores.AzureDevOps.Contratos;
using Microsoft.Extensions.Options;

namespace Infraestructure.Adaptadores.AzureDevOps;

/// <summary>Adaptador de salida: implementa el puerto de revisiones con la API REST de Azure DevOps.</summary>
internal sealed class RevisionesAzureDevOpsAdaptador(ClienteAzureDevOps cliente, IOptions<AzureDevOpsOpciones> opciones)
    : IRevisionesWorkItemPuerto
{
    public async Task<IReadOnlyList<RevisionWorkItem>> ObtenerRevisionesAsync(
        string organizacion, string proyecto, int workItemId, CancellationToken ct)
    {
        string ruta = $"{Uri.EscapeDataString(organizacion)}/{Uri.EscapeDataString(proyecto)}" +
                      $"/_apis/wit/workItems/{workItemId}/revisions?api-version={opciones.Value.VersionApi}";

        var respuesta = await cliente.GetAsync<RespuestaRevisiones>(ruta, $"El work item {workItemId}", ct);

        // Traducción del contrato externo al modelo del dominio.
        return respuesta.Value.Select(r => new RevisionWorkItem(
            r.Rev,
            r.Fields.AssignedTo?.DisplayName,
            r.Fields.ChangedBy?.DisplayName ?? string.Empty,
            r.Fields.ChangedDate,
            r.Fields.DueDate,
            r.Fields.History)).ToList();
    }
}

/// <summary>Adaptador de salida: implementa el puerto de consultas con WIQL de Azure DevOps.</summary>
internal sealed class ConsultasAzureDevOpsAdaptador(ClienteAzureDevOps cliente, IOptions<AzureDevOpsOpciones> opciones)
    : IConsultasWorkItemsPuerto
{
    public async Task<ResultadoConsulta> EjecutarConsultaGuardadaAsync(
        string organizacion, string proyecto, Guid consultaId, CancellationToken ct)
    {
        string ruta = $"{Base(organizacion, proyecto)}/_apis/wit/wiql/{consultaId}?api-version={opciones.Value.VersionApi}";
        return Mapear(await cliente.GetAsync<RespuestaWiql>(ruta, $"La consulta {consultaId}", ct));
    }

    public async Task<ResultadoConsulta> EjecutarWiqlAsync(string organizacion, string proyecto, string wiql, CancellationToken ct)
    {
        string ruta = $"{Base(organizacion, proyecto)}/_apis/wit/wiql?api-version={opciones.Value.VersionApi}";
        return Mapear(await cliente.PostAsync<RespuestaWiql>(ruta, new { query = wiql }, "La consulta WIQL", ct));
    }

    private static string Base(string organizacion, string proyecto) =>
        $"{Uri.EscapeDataString(organizacion)}/{Uri.EscapeDataString(proyecto)}";

    private static ResultadoConsulta Mapear(RespuestaWiql r) => new(r.AsOf, r.WorkItems.Select(w => w.Id).ToList());
}

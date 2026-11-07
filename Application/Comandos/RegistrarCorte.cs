using Application.Consultas;
using Application.Cqrs;
using Application.Dtos;
using Application.Puertos.Salida;
using Application.Servicios;
using Domain.Auditoria;

namespace Application.Comandos;

/// <summary>
/// COMANDO (lado de escritura): congela la trazabilidad actual de un work item como evidencia.
/// Lee Azure DevOps sin caché para registrar el estado real en ese instante.
/// </summary>
public sealed record RegistrarCorteComando(string Organizacion, string Proyecto, int WorkItemId, string Motivo)
    : IComando<CorteRegistradoDto>;

internal sealed class RegistrarCorteValidador : IValidador<RegistrarCorteComando>
{
    public IEnumerable<(string, string)> Validar(RegistrarCorteComando c)
    {
        foreach (var e in Reglas.OrganizacionYProyecto(c.Organizacion, c.Proyecto).Concat(Reglas.WorkItem(c.WorkItemId)))
            yield return e;
        var longitud = c.Motivo?.Trim().Length ?? 0;
        if (longitud is < CorteTrazabilidad.LongitudMinimaMotivo or > CorteTrazabilidad.LongitudMaximaMotivo)
            yield return ("motivo", $"El motivo debe tener entre {CorteTrazabilidad.LongitudMinimaMotivo} y {CorteTrazabilidad.LongitudMaximaMotivo} caracteres.");
    }
}

internal sealed class RegistrarCorteManejador(
    GeneradorTrazabilidad generador, ICortesTrazabilidadRepositorio cortes, IReloj reloj)
    : IManejador<RegistrarCorteComando, CorteRegistradoDto>
{
    public async Task<CorteRegistradoDto> ManejarAsync(RegistrarCorteComando c, CancellationToken ct)
    {
        var (_, periodos) = await generador.GenerarAsync(c.Organizacion, c.Proyecto, c.WorkItemId, ct);
        var corte = CorteTrazabilidad.Registrar(c.Organizacion, c.Proyecto, c.WorkItemId, c.Motivo, periodos, reloj.AhoraUtc);
        await cortes.GuardarAsync(corte, ct);
        return new CorteRegistradoDto(corte.Id, corte.FechaCorteUtc, corte.Huella);
    }
}

using System.Security.Cryptography;
using System.Text;
using Domain.Trazabilidad;

namespace Domain.Auditoria;

/// <summary>
/// Agregado del LADO DE ESCRITURA: una fotografía inmutable de la trazabilidad de un work item
/// en un momento dado. Sirve como evidencia para auditoría o supervisión contractual, porque el
/// historial en Azure DevOps puede cambiar. La huella SHA-256 permite detectar alteraciones.
/// </summary>
public sealed class CorteTrazabilidad
{
    public const int LongitudMinimaMotivo = 5;
    public const int LongitudMaximaMotivo = 300;

    public Guid Id { get; }
    public string Organizacion { get; }
    public string Proyecto { get; }
    public int WorkItemId { get; }
    public DateTime FechaCorteUtc { get; }
    public string Motivo { get; }
    public IReadOnlyList<PeriodoAsignacion> Periodos { get; }
    public string Huella { get; }

    private CorteTrazabilidad(Guid id, string organizacion, string proyecto, int workItemId, DateTime fechaCorteUtc,
        string motivo, IReadOnlyList<PeriodoAsignacion> periodos, string huella)
    {
        Id = id; Organizacion = organizacion; Proyecto = proyecto; WorkItemId = workItemId;
        FechaCorteUtc = fechaCorteUtc; Motivo = motivo; Periodos = periodos; Huella = huella;
    }

    /// <summary>Fábrica: aplica las reglas de negocio y calcula la huella.</summary>
    public static CorteTrazabilidad Registrar(string organizacion, string proyecto, int workItemId, string motivo,
        IReadOnlyList<PeriodoAsignacion> periodos, DateTime fechaCorteUtc)
    {
        motivo = (motivo ?? string.Empty).Trim();
        if (motivo.Length is < LongitudMinimaMotivo or > LongitudMaximaMotivo)
            throw new ReglaDominioException($"El motivo del corte debe tener entre {LongitudMinimaMotivo} y {LongitudMaximaMotivo} caracteres.");
        if (periodos is null || periodos.Count == 0)
            throw new ReglaDominioException("No se puede registrar un corte de un work item sin historial de asignaciones.");

        var id = Guid.NewGuid();
        return new CorteTrazabilidad(id, organizacion, proyecto, workItemId, fechaCorteUtc, motivo, periodos,
            CalcularHuella(workItemId, fechaCorteUtc, motivo, periodos));
    }

    /// <summary>Reconstruye un corte ya persistido (no recalcula la huella: la conserva para verificarla).</summary>
    public static CorteTrazabilidad Reconstituir(Guid id, string organizacion, string proyecto, int workItemId,
        DateTime fechaCorteUtc, string motivo, IReadOnlyList<PeriodoAsignacion> periodos, string huella)
        => new(id, organizacion, proyecto, workItemId, fechaCorteUtc, motivo, periodos, huella);

    /// <summary>¿El contenido coincide con la huella con la que se registró?</summary>
    public bool EsIntegro() => CalcularHuella(WorkItemId, FechaCorteUtc, Motivo, Periodos) == Huella;

    private static string CalcularHuella(int workItemId, DateTime fecha, string motivo, IEnumerable<PeriodoAsignacion> periodos)
    {
        var texto = new StringBuilder()
            .Append(workItemId).Append('|').Append(fecha.ToUniversalTime().ToString("O")).Append('|').Append(motivo);
        foreach (var p in periodos)
        {
            texto.Append('|').Append(p.Responsable)
                 .Append('|').Append(p.Desde.ToUniversalTime().ToString("O"))
                 .Append('|').Append(p.Hasta?.ToUniversalTime().ToString("O"))
                 .Append('|').Append(p.FechaLimite?.ToUniversalTime().ToString("O"))
                 .Append('|').Append(string.Join("¦", p.Comentarios));
        }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(texto.ToString())));
    }
}

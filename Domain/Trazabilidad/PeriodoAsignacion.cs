namespace Domain.Trazabilidad;

/// <summary>
/// Intervalo continuo en el que un work item estuvo a cargo de un mismo responsable.
/// </summary>
public sealed record PeriodoAsignacion(
    string Responsable,
    DateTime Desde,
    DateTime? Hasta,
    DateTime? FechaLimite,
    IReadOnlyList<string> Comentarios)
{
    public const string SinAsignar = "(Sin asignar)";

    /// <summary>El periodo sigue abierto: el work item aún está a cargo de este responsable.</summary>
    public bool Vigente => Hasta is null;

    /// <summary>Duración del periodo; si está vigente se mide hasta <paramref name="ahora"/>.</summary>
    public TimeSpan Duracion(DateTime ahora) => (Hasta ?? ahora) - Desde;

    /// <summary>El responsable entregó (o sigue) después de la fecha límite.</summary>
    public bool Vencido(DateTime ahora) => FechaLimite is { } limite && (Hasta ?? ahora) > limite;
}

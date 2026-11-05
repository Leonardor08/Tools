namespace Domain.Trazabilidad;

/// <summary>
/// Una revisión (versión) de un work item, expresada en términos del dominio.
/// No conoce el JSON de Azure DevOps: el adaptador de infraestructura la construye.
/// </summary>
public sealed record RevisionWorkItem(
    int Numero,
    string? AsignadoA,
    string ModificadoPor,
    DateTime FechaCambio,
    DateTime? FechaLimite,
    string? Comentario);

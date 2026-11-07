namespace Application.Dtos;

public sealed record PeriodoAsignacionDto(
    string Responsable,
    DateTime Desde,
    DateTime? Hasta,
    bool Vigente,
    double HorasAsignado,
    DateTime? FechaLimite,
    bool Vencido,
    IReadOnlyList<string> Comentarios);

public sealed record TrazabilidadWorkItemDto(
    int WorkItemId,
    int TotalRevisiones,
    IReadOnlyList<PeriodoAsignacionDto> Periodos);

public sealed record ResultadoConsultaDto(DateTime FechaCorte, int Total, IReadOnlyList<int> WorkItemIds);

public sealed record CorteRegistradoDto(Guid Id, DateTime FechaCorteUtc, string Huella);

public sealed record CorteResumenDto(Guid Id, int WorkItemId, DateTime FechaCorteUtc, string Motivo,
    int TotalPeriodos, string Huella, bool Integro);

public sealed record CorteDetalleDto(Guid Id, string Organizacion, string Proyecto, int WorkItemId, DateTime FechaCorteUtc,
    string Motivo, string Huella, bool Integro, IReadOnlyList<PeriodoAsignacionDto> Periodos);

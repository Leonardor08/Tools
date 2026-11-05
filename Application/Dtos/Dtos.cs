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

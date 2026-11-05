namespace Domain.Trazabilidad;

/// <summary>
/// Servicio de dominio: convierte el historial de revisiones en periodos de asignación.
/// Regla: revisiones consecutivas con el mismo responsable forman UN periodo; el periodo
/// termina cuando cambia el responsable. Se atribuyen al periodo los comentarios que el
/// propio responsable escribió mientras lo tenía asignado.
/// </summary>
public static class CalculadoraTrazabilidad
{
    public static IReadOnlyList<PeriodoAsignacion> Calcular(IEnumerable<RevisionWorkItem> revisiones)
    {
        ArgumentNullException.ThrowIfNull(revisiones);

        var ordenadas = revisiones.OrderBy(r => r.Numero).ToList();
        var periodos = new List<PeriodoAsignacion>();
        int inicio = 0;

        while (inicio < ordenadas.Count)
        {
            string responsable = Normalizar(ordenadas[inicio].AsignadoA);
            int fin = inicio;
            while (fin + 1 < ordenadas.Count && Normalizar(ordenadas[fin + 1].AsignadoA) == responsable)
                fin++;

            var tramo = ordenadas.GetRange(inicio, fin - inicio + 1);
            DateTime? hasta = fin + 1 < ordenadas.Count ? ordenadas[fin + 1].FechaCambio : null;

            var comentarios = tramo
                .Where(r => r.ModificadoPor == responsable && !string.IsNullOrWhiteSpace(r.Comentario))
                .Select(r => r.Comentario!.Trim())
                .ToList();

            periodos.Add(new PeriodoAsignacion(
                responsable,
                tramo[0].FechaCambio,
                hasta,
                tramo.Select(r => r.FechaLimite).LastOrDefault(f => f is not null),
                comentarios));

            inicio = fin + 1;
        }

        return periodos;
    }

    private static string Normalizar(string? responsable) =>
        string.IsNullOrWhiteSpace(responsable) ? PeriodoAsignacion.SinAsignar : responsable.Trim();
}

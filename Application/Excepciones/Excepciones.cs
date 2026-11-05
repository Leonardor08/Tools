namespace Application.Excepciones;

/// <summary>Los datos de entrada no cumplen las reglas del caso de uso (HTTP 400).</summary>
public sealed class ValidacionException(IReadOnlyDictionary<string, string[]> errores)
    : Exception("La solicitud contiene datos inválidos.")
{
    public IReadOnlyDictionary<string, string[]> Errores { get; } = errores;

    public static ValidacionException De(string campo, string mensaje) =>
        new(new Dictionary<string, string[]> { [campo] = [mensaje] });
}

/// <summary>El recurso solicitado no existe en el sistema externo (HTTP 404).</summary>
public sealed class RecursoNoEncontradoException(string mensaje) : Exception(mensaje);

/// <summary>El sistema externo (Azure DevOps) falló o no está disponible (HTTP 502).</summary>
public sealed class ServicioExternoException(string mensaje, Exception? interna = null) : Exception(mensaje, interna);

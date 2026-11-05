using Application.Excepciones;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ApiTools.Infraestructura;

/// <summary>Traduce las excepciones de la aplicación a respuestas RFC 7807 (ProblemDetails).</summary>
public sealed class ManejadorExcepciones(IProblemDetailsService problemas, ILogger<ManejadorExcepciones> log) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception excepcion, CancellationToken ct)
    {
        ProblemDetails detalle = excepcion switch
        {
            ValidacionException v => new ValidationProblemDetails(v.Errores.ToDictionary(e => e.Key, e => e.Value))
                { Status = StatusCodes.Status400BadRequest, Title = v.Message },
            RecursoNoEncontradoException n => new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = n.Message },
            ServicioExternoException s => new ProblemDetails { Status = StatusCodes.Status502BadGateway, Title = s.Message },
            _ => new ProblemDetails { Status = StatusCodes.Status500InternalServerError, Title = "Error inesperado." }
        };

        if (detalle.Status >= 500) log.LogError(excepcion, "Error procesando {Ruta}", contexto.Request.Path);

        contexto.Response.StatusCode = detalle.Status!.Value;
        return await problemas.TryWriteAsync(new ProblemDetailsContext { HttpContext = contexto, ProblemDetails = detalle, Exception = excepcion });
    }
}

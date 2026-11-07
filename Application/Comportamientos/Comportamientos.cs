using System.Diagnostics;
using Application.Cqrs;
using Application.Excepciones;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Comportamientos;

/// <summary>1.º Registro y medición de tiempo de cada solicitud (observabilidad).</summary>
internal sealed class ComportamientoRegistro<TSolicitud, TRespuesta>(ILogger<ComportamientoRegistro<TSolicitud, TRespuesta>> log)
    : IComportamiento<TSolicitud, TRespuesta> where TSolicitud : ISolicitud<TRespuesta>
{
    private static readonly TimeSpan UmbralLento = TimeSpan.FromSeconds(3);

    public async Task<TRespuesta> ManejarAsync(TSolicitud solicitud, SiguientePaso<TRespuesta> siguiente, CancellationToken ct)
    {
        string tipo = solicitud is IComando<TRespuesta> ? "Comando" : "Consulta";
        var reloj = Stopwatch.StartNew();
        try
        {
            var respuesta = await siguiente();
            if (reloj.Elapsed > UmbralLento)
                log.LogWarning("{Tipo} {Nombre} lenta: {Ms} ms", tipo, typeof(TSolicitud).Name, reloj.ElapsedMilliseconds);
            else
                log.LogInformation("{Tipo} {Nombre} atendida en {Ms} ms", tipo, typeof(TSolicitud).Name, reloj.ElapsedMilliseconds);
            return respuesta;
        }
        catch (Exception ex)
        {
            log.LogWarning("{Tipo} {Nombre} falló tras {Ms} ms: {Error}", tipo, typeof(TSolicitud).Name, reloj.ElapsedMilliseconds, ex.Message);
            throw;
        }
    }
}

/// <summary>2.º Validación: ninguna solicitud inválida llega al manejador ni a Azure DevOps.</summary>
internal sealed class ComportamientoValidacion<TSolicitud, TRespuesta>(IEnumerable<IValidador<TSolicitud>> validadores)
    : IComportamiento<TSolicitud, TRespuesta> where TSolicitud : ISolicitud<TRespuesta>
{
    public Task<TRespuesta> ManejarAsync(TSolicitud solicitud, SiguientePaso<TRespuesta> siguiente, CancellationToken ct)
    {
        var errores = validadores.SelectMany(v => v.Validar(solicitud))
            .GroupBy(e => e.Campo)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Mensaje).ToArray());

        if (errores.Count > 0) throw new ValidacionException(errores);
        return siguiente();
    }
}

/// <summary>
/// 3.º Caché solo para CONSULTAS marcadas como cacheables: reduce llamadas a Azure DevOps
/// (límites de uso) y la latencia. Los comandos nunca pasan por la caché.
/// </summary>
internal sealed class ComportamientoCache<TSolicitud, TRespuesta>(IMemoryCache cache)
    : IComportamiento<TSolicitud, TRespuesta> where TSolicitud : ISolicitud<TRespuesta>
{
    public async Task<TRespuesta> ManejarAsync(TSolicitud solicitud, SiguientePaso<TRespuesta> siguiente, CancellationToken ct)
    {
        if (solicitud is not IConsultaCacheable cacheable || solicitud is IComando<TRespuesta>)
            return await siguiente();

        string clave = $"{typeof(TSolicitud).Name}:{cacheable.ClaveCache}";
        if (cache.TryGetValue(clave, out TRespuesta? guardada) && guardada is not null)
            return guardada;

        var respuesta = await siguiente();
        cache.Set(clave, respuesta, cacheable.DuracionCache);
        return respuesta;
    }
}

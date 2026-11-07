using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Cqrs;

/// <summary>
/// Despacha cada solicitud a su único manejador, envuelto por los comportamientos registrados
/// (el primero registrado es el más externo).
/// </summary>
internal sealed class Mediador(IServiceProvider proveedor) : IMediador
{
    private static readonly ConcurrentDictionary<Type, object> Envoltorios = new();

    public Task<TRespuesta> EnviarAsync<TRespuesta>(ISolicitud<TRespuesta> solicitud, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(solicitud);
        var envoltorio = (EnvoltorioBase<TRespuesta>)Envoltorios.GetOrAdd(solicitud.GetType(), tipo =>
            Activator.CreateInstance(typeof(Envoltorio<,>).MakeGenericType(tipo, typeof(TRespuesta)))!);
        return envoltorio.EjecutarAsync(solicitud, proveedor, ct);
    }

    private abstract class EnvoltorioBase<TRespuesta>
    {
        public abstract Task<TRespuesta> EjecutarAsync(object solicitud, IServiceProvider proveedor, CancellationToken ct);
    }

    private sealed class Envoltorio<TSolicitud, TRespuesta> : EnvoltorioBase<TRespuesta>
        where TSolicitud : ISolicitud<TRespuesta>
    {
        public override Task<TRespuesta> EjecutarAsync(object solicitud, IServiceProvider proveedor, CancellationToken ct)
        {
            var mensaje = (TSolicitud)solicitud;
            var manejador = proveedor.GetRequiredService<IManejador<TSolicitud, TRespuesta>>();

            SiguientePaso<TRespuesta> paso = () => manejador.ManejarAsync(mensaje, ct);
            foreach (var comportamiento in proveedor.GetServices<IComportamiento<TSolicitud, TRespuesta>>().Reverse())
            {
                var siguiente = paso;
                paso = () => comportamiento.ManejarAsync(mensaje, siguiente, ct);
            }
            return paso();
        }
    }
}

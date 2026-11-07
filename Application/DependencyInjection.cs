using System.Reflection;
using Application.Comportamientos;
using Application.Cqrs;
using Application.Servicios;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    /// <summary>Registra el mediador, los manejadores, los validadores y el pipeline de comportamientos.</summary>
    public static IServiceCollection AgregarAplicacion(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IMediador, Mediador>();
        services.AddScoped<GeneradorTrazabilidad>();

        // Orden del pipeline (externo → interno): registro → validación → caché → manejador.
        services.AddScoped(typeof(IComportamiento<,>), typeof(ComportamientoRegistro<,>));
        services.AddScoped(typeof(IComportamiento<,>), typeof(ComportamientoValidacion<,>));
        services.AddScoped(typeof(IComportamiento<,>), typeof(ComportamientoCache<,>));

        // Descubrimiento de manejadores y validadores en este ensamblado.
        RegistrarImplementaciones(services, typeof(IManejador<,>));
        RegistrarImplementaciones(services, typeof(IValidador<>));
        return services;
    }

    private static void RegistrarImplementaciones(IServiceCollection services, Type interfazGenerica)
    {
        var tipos = Assembly.GetExecutingAssembly().GetTypes().Where(t => t is { IsClass: true, IsAbstract: false });
        foreach (var tipo in tipos)
            foreach (var interfaz in tipo.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfazGenerica))
                services.AddScoped(interfaz, tipo);
    }
}

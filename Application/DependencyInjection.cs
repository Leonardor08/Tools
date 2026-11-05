using Application.CasosDeUso;
using Application.Puertos.Entrada;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    /// <summary>Registra los casos de uso (puertos de entrada) de la aplicación.</summary>
    public static IServiceCollection AgregarAplicacion(this IServiceCollection services)
    {
        services.AddScoped<IObtenerTrazabilidadWorkItem, ObtenerTrazabilidadWorkItem>();
        services.AddScoped<IEjecutarConsultaGuardada, EjecutarConsultaGuardada>();
        services.AddScoped<IConsultarHistoriasPorResponsable, ConsultarHistoriasPorResponsable>();
        return services;
    }
}

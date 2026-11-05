using System.Net.Http.Headers;
using System.Text;
using Application.Puertos.Salida;
using Infraestructure.Adaptadores.AzureDevOps;
using Infraestructure.Adaptadores.Html;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infraestructure;

public static class DependencyInjection
{
    /// <summary>Conecta los puertos de salida con sus adaptadores concretos.</summary>
    public static IServiceCollection AgregarInfraestructura(this IServiceCollection services, IConfiguration configuracion)
    {
        services.AddOptions<AzureDevOpsOpciones>()
            .Bind(configuracion.GetSection(AzureDevOpsOpciones.Seccion))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<ClienteAzureDevOps>((sp, http) =>
        {
            var o = sp.GetRequiredService<IOptions<AzureDevOpsOpciones>>().Value;
            http.BaseAddress = new Uri(o.UrlBase.TrimEnd('/') + "/");
            http.Timeout = TimeSpan.FromSeconds(o.TiempoEsperaSegundos);
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{o.Pat}")));
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddScoped<IRevisionesWorkItemPuerto, RevisionesAzureDevOpsAdaptador>();
        services.AddScoped<IConsultasWorkItemsPuerto, ConsultasAzureDevOpsAdaptador>();
        services.AddSingleton<ILimpiadorHtml, LimpiadorHtmlAgilityPack>();
        services.AddSingleton<IReloj, RelojSistema>();
        return services;
    }
}

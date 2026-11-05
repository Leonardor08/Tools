using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Application.Excepciones;

namespace Infraestructure.Adaptadores.AzureDevOps;

/// <summary>
/// HttpClient tipado: la autenticación y la dirección base se configuran UNA vez en el registro
/// (antes se mutaban los encabezados del cliente compartido en cada llamada). Traduce errores HTTP
/// a excepciones de la aplicación para que el dominio no dependa de HttpClient.
/// </summary>
internal sealed class ClienteAzureDevOps(HttpClient http)
{
    public async Task<T> GetAsync<T>(string ruta, string recurso, CancellationToken ct)
        => await LeerAsync<T>(await EnviarAsync(() => http.GetAsync(ruta, ct)), recurso, ct);

    public async Task<T> PostAsync<T>(string ruta, object cuerpo, string recurso, CancellationToken ct)
    {
        // StringContent envía Content-Length (sin "chunked"), como hacía la versión original contra Azure DevOps.
        using var contenido = new StringContent(JsonSerializer.Serialize(cuerpo), Encoding.UTF8, "application/json");
        return await LeerAsync<T>(await EnviarAsync(() => http.PostAsync(ruta, contenido, ct)), recurso, ct);
    }

    private static async Task<HttpResponseMessage> EnviarAsync(Func<Task<HttpResponseMessage>> envio)
    {
        try { return await envio(); }
        catch (HttpRequestException ex) { throw new ServicioExternoException("No fue posible conectar con Azure DevOps.", ex); }
        catch (TaskCanceledException ex) { throw new ServicioExternoException("Azure DevOps no respondió a tiempo.", ex); }
    }

    private static async Task<T> LeerAsync<T>(HttpResponseMessage respuesta, string recurso, CancellationToken ct)
    {
        using (respuesta)
        {
            switch (respuesta.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new RecursoNoEncontradoException($"{recurso} no existe en Azure DevOps.");
                case HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden or HttpStatusCode.NonAuthoritativeInformation:
                    // Azure DevOps responde 203 con una página de inicio de sesión cuando el PAT es inválido.
                    throw new ServicioExternoException("Azure DevOps rechazó las credenciales: revise el PAT configurado.");
            }
            if (!respuesta.IsSuccessStatusCode)
                throw new ServicioExternoException($"Azure DevOps respondió {(int)respuesta.StatusCode} al consultar {recurso}.");

            try
            {
                return await respuesta.Content.ReadFromJsonAsync<T>(ct)
                       ?? throw new ServicioExternoException($"Azure DevOps devolvió una respuesta vacía para {recurso}.");
            }
            catch (JsonException ex)
            {
                throw new ServicioExternoException($"La respuesta de Azure DevOps para {recurso} no tiene el formato esperado.", ex);
            }
        }
    }
}

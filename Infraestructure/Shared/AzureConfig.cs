using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infraestructure.Shared;

public class AzureConfig(HttpClient httpClient, IConfiguration config)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;

    public string ReturnPat() 
        => Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_config["AzureDevOps:PAT"]}"));

    public static HttpContent ReturnContentTypeHeader(string wiqlQuery)
        => new StringContent(JsonSerializer.Serialize(new { query = wiqlQuery }), Encoding.UTF8, "application/json" );
    public void AddAuthorizationHeader()
        => _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", ReturnPat());

    public JsonSerializerOptions ReturnJsonOptions()
        => new() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.Never };
}

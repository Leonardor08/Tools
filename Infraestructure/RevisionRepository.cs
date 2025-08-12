using Application.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infraestructure;

public class RevisionRepository(HttpClient httpClient, IConfiguration config) : IRevisionRepository
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;
    public async Task<Revision> GetRevisionsByWorkItem(string organization, string project, string workItemId, CancellationToken cancellationToken)
    {
        Revision revision;
        string url = _config["AzureEndpoints:RevisionsByWorkItem"]!
            .Replace("{organization}", organization)
            .Replace("{project}", project)
            .Replace("{id}", workItemId);
        string patEncoded = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_config["AzureDevOps:PAT"]}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", patEncoded);
        HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };
        string json = await response.Content.ReadAsStringAsync(cancellationToken);            
        try
        {
            revision = JsonSerializer.Deserialize<Revision>(json, jsonOptions) ?? throw new Exception("Api no retorna valor");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error de deserialización: {ex.Message}");
            Console.WriteLine($"Ruta del error: {ex.Path}");
            throw;
        }

        return revision;
    }       
}
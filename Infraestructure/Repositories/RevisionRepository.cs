using Application.Interfaces;
using Domain.Models;
using Infraestructure.Shared;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Infraestructure.Repositories;

public class RevisionRepository(HttpClient httpClient, IConfiguration config) : IRevisionRepository
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;
    public async Task<Revision> GetRevisionsByWorkItem(string organization, string project, string workItemId, CancellationToken cancellationToken)
    {        
        Revision revision;
        AzureConfig azure = new(_httpClient, _config);
        azure.AddAuthorizationHeader();


        string url = _config["AzureEndpoints:RevisionsByWorkItem"]!
            .Replace("{organization}", organization)
            .Replace("{project}", project)
            .Replace("{id}", workItemId);

        HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        try
        {
            revision = JsonSerializer.Deserialize<Revision>(json, azure.ReturnJsonOptions()) ?? 
                throw new Exception("Api no retorna valor");
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
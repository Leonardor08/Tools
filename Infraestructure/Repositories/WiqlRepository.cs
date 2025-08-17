using Application.Interfaces;
using Domain.Models;
using Infraestructure.Shared;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using static Domain.Enums;

namespace Infraestructure.Repositories;

public class WiqlRepository(HttpClient httpClient, IConfiguration config) : IWiqlRepository
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;
    public async Task<Query> GetQueryByIdAsync(string organization, string project, string idQuery)
    {
        Query query = new();

        AzureConfig azure = new(_httpClient, _config);

        azure.AddAuthorizationHeader();

        string url = _config["AzureEndpoints:QueryById"]!
            .Replace("{organization}", organization)
            .Replace("{project}", project)
            .Replace("{id}", idQuery);

        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();

        try
        {
            query = JsonSerializer.Deserialize<Query>(json, azure.ReturnJsonOptions()) ??
                throw new Exception("Api no retorna valor");
        }

        catch (JsonException ex)
        {
            Console.WriteLine($"Error de deserialización: {ex.Message}");
            Console.WriteLine($"Ruta del error: {ex.Path}");
            throw;
        }

        return query;
    }

    public async Task<Query> GetWiqlAsync(string organization, string project, Devs Dev, string wiqlQuery)
    {
        Query query = new();

        AzureConfig azure = new(_httpClient, _config);

        azure.AddAuthorizationHeader();

        string url = _config["AzureEndpoints:QueryByWiql"]!
            .Replace("{organization}", organization)
            .Replace("{project}", project);

        HttpResponseMessage response = await _httpClient.PostAsync(url, AzureConfig.ReturnContentTypeHeader(wiqlQuery));
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();

        try
        {
            query = JsonSerializer.Deserialize<Query>(json, azure.ReturnJsonOptions()) ??
                throw new Exception("Api no retorna valor");
        }

        catch (JsonException ex)
        {
            Console.WriteLine($"Error de deserialización: {ex.Message}");
            Console.WriteLine($"Ruta del error: {ex.Path}");
            throw;
        }

        return query;
    }
}

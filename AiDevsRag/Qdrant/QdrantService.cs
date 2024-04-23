using AiDevsRag.Config;
using AiDevsRag.OpenAI;
using AiDevsRag.Qdrant.Embeddings;
using AiDevsRag.Qdrant.Search;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AiDevsRag.Qdrant;

public sealed class QdrantService : IQdrantService
{
    private readonly string _qdrantUrl;
    private readonly string _collectionName;

    public QdrantService(IOptions<QdrantConfig> config)
    {
        _qdrantUrl = config.Value.BaseUrl;
        _collectionName = config.Value.CollectionName;
    }
    
    public async Task CreateCollectionAsync()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_qdrantUrl}/{_collectionName}");

        string json = JsonSerializer.Serialize(new QdrantCollectionRequest(new Vectors()), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        });
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> CheckIfCollectionExistsAsync()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_qdrantUrl}/{_collectionName}");
        var response = await client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<QdrantCollectionResponse?> GetCollectionInfoAsync()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_qdrantUrl}/{_collectionName}");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<QdrantCollectionResponse>();
        return result;
    }
    
    public async Task UpsertPointsAsync(QdrantPoints points, 
        CancellationToken cancellationToken) // TODO: move collection name to appsettings
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_qdrantUrl}/{_collectionName}/points");

        string json = JsonSerializer.Serialize(points, OpenAiService.JsonOptions); // TODO: move json options to separate class
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;
    
        var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine(responseContent);
        }
        response.EnsureSuccessStatusCode();
    }

    public async Task<QdrantSearchResponse?> SearchAsync(QdrantSearchRequest searchRequest,
        CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_qdrantUrl}/{_collectionName}/points/search");

        string json = JsonSerializer.Serialize(searchRequest, OpenAiService.JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;
    
        var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine(responseContent);
        }
        response.EnsureSuccessStatusCode();
    
        return await response.Content.ReadFromJsonAsync<QdrantSearchResponse>(cancellationToken);
    }

}
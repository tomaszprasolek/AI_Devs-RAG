using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AiDevsRag.Qdrant;

public interface IQdrantService
{
    Task CreateCollectionAsync(string collectionName);
    Task<bool> CheckIfCollectionExistsAsync(string collectionName);
    Task<QdrantCollectionResponse?> GetCollectionInfoAsync(string collectionName);
}

public sealed class QdrantService : IQdrantService
{
    private readonly string _qdrantUrl = "http://localhost:6333/collections";
    
    public async Task CreateCollectionAsync(string collectionName)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_qdrantUrl}/{collectionName}");

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

    public async Task<bool> CheckIfCollectionExistsAsync(string collectionName)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_qdrantUrl}/{collectionName}");
        var response = await client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<QdrantCollectionResponse?> GetCollectionInfoAsync(string collectionName)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_qdrantUrl}/{collectionName}");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<QdrantCollectionResponse>();
        return result;
    }
}
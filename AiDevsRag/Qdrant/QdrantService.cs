using AiDevsRag.OpenAI;
using AiDevsRag.Qdrant.Embeddings;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AiDevsRag.Qdrant;

public interface IQdrantService
{
    Task CreateCollectionAsync(string collectionName);
    Task<bool> CheckIfCollectionExistsAsync(string collectionName);
    Task<QdrantCollectionResponse?> GetCollectionInfoAsync(string collectionName);

    Task UpsertPointsAsync(string collectionName,
        QdrantPoints points,
        CancellationToken cancellationToken);
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
    
    public async Task UpsertPointsAsync(string collectionName, QdrantPoints points, 
        CancellationToken cancellationToken) // TODO: move collection name to appsettings
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_qdrantUrl}/{collectionName}/points");

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

}
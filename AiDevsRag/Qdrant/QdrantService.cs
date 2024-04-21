using System.Text;
using System.Text.Json;

namespace AiDevsRag.Qdrant;

public sealed class QdrantService
{
    private readonly string _qdrantUrl = "http://localhost:6333/collections";
    
    public async Task CreateCollection(string collectionName)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_qdrantUrl}/{collectionName}");

        string json = JsonSerializer.Serialize(new QdrantCollection(new Vectors()), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        });
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> CheckIfCollectionExists(string collectionName)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_qdrantUrl}/{collectionName}");
        var response = await client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}
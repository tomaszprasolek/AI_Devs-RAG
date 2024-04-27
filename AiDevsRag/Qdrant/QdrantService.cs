using AiDevsRag.Config;
using AiDevsRag.OpenAI;
using AiDevsRag.Qdrant.Embeddings;
using AiDevsRag.Qdrant.Scroll;
using AiDevsRag.Qdrant.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AiDevsRag.Qdrant;

public sealed class QdrantService : IQdrantService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<QdrantService> _logger;
    private readonly string _collectionName;

    public QdrantService(
        HttpClient httpClient,
        IOptions<QdrantConfig> config, 
        ILogger<QdrantService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _collectionName = config.Value.CollectionName;
    }
    
    public async Task CreateCollectionAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"collections/{_collectionName}");

        string json = JsonSerializer.Serialize(new QdrantCollectionRequest(new Vectors()), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        });
        _logger.LogDebug("Request payload: {Payload}",json);
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;
        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug(responseContent);
        }
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> CheckIfCollectionExistsAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"collections/{_collectionName}");
        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug(responseContent);
            return false;
        }

        return true;
    }

    public async Task<QdrantCollectionResponse?> GetCollectionInfoAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"collections/{_collectionName}");
        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug(responseContent);
        }
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<QdrantCollectionResponse>(cancellationToken: cancellationToken);
        return result;
    }
    
    public async Task UpsertPointsAsync(QdrantPoints points, 
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"collections/{_collectionName}/points");

        string json = JsonSerializer.Serialize(points, OpenAiService.JsonOptions); // TODO: move json options to separate class
        _logger.LogDebug("Request payload: {Payload}",json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;
    
        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug(responseContent);
        }
        response.EnsureSuccessStatusCode();
    }

    public async Task<QdrantSearchResponse?> SearchAsync(QdrantSearchRequest searchRequest,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"collections/{_collectionName}/points/search");

        string json = JsonSerializer.Serialize(searchRequest, OpenAiService.JsonOptions);
        _logger.LogDebug("Request payload: {Payload}",json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;
    
        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine(responseContent);
        }
        response.EnsureSuccessStatusCode();
    
        return await response.Content.ReadFromJsonAsync<QdrantSearchResponse>(cancellationToken);
    }

    public async Task<bool> CheckIfDocumentExistAsync(string documentName,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"collections/{_collectionName}/points/scroll");

        string json = """
                      {
                          "filter": {
                              "must": [
                                  {
                                      "key": "DocumentName",
                                      "match": {
                                          "value": "##DocumentName##"
                                      }
                                  }
                              ]
                          },
                          "limit": 1
                      }
                      """;
        json = json.Replace("##DocumentName##", documentName);
        _logger.LogDebug("Request payload: {Payload}",json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;
    
        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine(responseContent);
        }
        response.EnsureSuccessStatusCode();
    
        ScrollResponse? result = await response.Content.ReadFromJsonAsync<ScrollResponse>(cancellationToken);
        if (result is null)
            return false;
        
        return result.Result.Points.Count != 0;
    }

}
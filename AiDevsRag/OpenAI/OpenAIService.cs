using AiDevsRag.OpenAI.Embeddings;
using AiDevsRag.OpenAI.Request;
using AiDevsRag.OpenAI.Response;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace AiDevsRag.OpenAI;

public sealed class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAiService> _logger;

    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public OpenAiService(
        HttpClient httpClient,
        ILogger<OpenAiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GptResponse> ChatAsync(GptPrompt prompt,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions");
        
        string json = JsonSerializer.Serialize(prompt, JsonOptions);
        _logger.LogDebug("Request payload: {Payload}",json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug(responseContent);
        }
        response.EnsureSuccessStatusCode();
        return response.Content
            .ReadFromJsonAsync<GptResponse>(JsonOptions, cancellationToken).Result!;
    }

    public async Task<GptResponse?> ChatWithFunctionAsync(GptPrompt prompt,
        string functionJson,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions");
        
        string json = JsonSerializer.Serialize(prompt, JsonOptions);
        // Parse the JSON string into a JsonNode
        JsonNode? jsonNode = JsonNode.Parse(json);
        // Cast the JsonNode to a JsonObject
        var jsonObject = (JsonObject) jsonNode!;

        jsonObject.Add("tools", JsonNode.Parse(functionJson));
        jsonObject.Add("tool_choice", "auto");

        json = jsonObject.ToJsonString(JsonOptions);

        _logger.LogDebug("Request payload: {Payload}",json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug(responseContent);
        }
        response.EnsureSuccessStatusCode();
        
        return await response.Content
            .ReadFromJsonAsync<GptResponse>(JsonOptions, cancellationToken);
    }

    public async Task<Embedding?> GenerateEmbeddingsAsync(EmbeddingRequest embeddingRequest, 
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "v1/embeddings");
        
        string json = JsonSerializer.Serialize(embeddingRequest, JsonOptions);
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

        return await response.Content.ReadFromJsonAsync<Embedding>(cancellationToken: cancellationToken);
    }
}
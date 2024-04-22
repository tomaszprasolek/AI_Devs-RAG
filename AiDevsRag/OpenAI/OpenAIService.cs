using AiDevsRag.OpenAI.Embeddings;
using AiDevsRag.OpenAI.Request;
using AiDevsRag.OpenAI.Response;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace AiDevsRag.OpenAI;

public interface IOpenAiService
{
    Task<GptResponse> ChatAsync(GptPrompt prompt,
        CancellationToken cancellationToken);

    Task<GptResponse?> ChatWithFunctionAsync(GptPrompt prompt,
        string functionJson,
        CancellationToken cancellationToken);

    Task<Embedding?> GenerateEmbeddingsAsync(EmbeddingRequest embeddingRequest,
        CancellationToken cancellationToken);
}

public sealed class OpenAiService : IOpenAiService
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    private readonly string _apiKey;

    public OpenAiService(IOptions<OpenAiConfig> settings)
    {
        _apiKey = settings.Value.ApiKey;
    }

    public async Task<GptResponse> ChatAsync(GptPrompt prompt,
        CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        string json = JsonSerializer.Serialize(prompt, JsonOptions);
        //Console.WriteLine(json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;

        HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        return response.Content
            .ReadFromJsonAsync<GptResponse>(JsonOptions, cancellationToken).Result!;
    }

    public async Task<GptResponse?> ChatWithFunctionAsync(GptPrompt prompt,
        string functionJson,
        CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");


        string json = JsonSerializer.Serialize(prompt, JsonOptions);
        // Parse the JSON string into a JsonNode
        JsonNode? jsonNode = JsonNode.Parse(json);
        // Cast the JsonNode to a JsonObject
        var jsonObject = (JsonObject) jsonNode!;

        jsonObject.Add("tools", JsonNode.Parse(functionJson));
        jsonObject.Add("tool_choice", "auto");

        json = jsonObject.ToJsonString(JsonOptions);

        //Console.WriteLine(json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;

        HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
        // if (!response.IsSuccessStatusCode)
        // {
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        Console.WriteLine(responseContent);
        // }
        response.EnsureSuccessStatusCode();
        
        return await response.Content
            .ReadFromJsonAsync<GptResponse>(JsonOptions, cancellationToken);
    }

    public async Task<Embedding?> GenerateEmbeddingsAsync(EmbeddingRequest embeddingRequest, 
        CancellationToken cancellationToken)
    {
        using HttpClient client = new();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/embeddings");
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");

        string json = JsonSerializer.Serialize(embeddingRequest, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;
    
        var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine(responseContent);
        }
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Embedding>(cancellationToken: cancellationToken);
    }
}
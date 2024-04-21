using AiDevsRag.OpenAI.Request;
using AiDevsRag.OpenAI.Response;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace AiDevsRag.OpenAI;

public sealed class OpenAiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };
    
    public async Task<GptResponse> ChatAsync(GptPrompt prompt,
        CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Add("Authorization", "Bearer");
        string json = JsonSerializer.Serialize(prompt, JsonOptions);
        //Console.WriteLine(json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;

        HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        return response.Content
            .ReadFromJsonAsync<GptResponse>(JsonOptions, cancellationToken: cancellationToken).Result!;
    }
    
    public async Task<GptResponse> ChatWithFunctionAsync(GptPrompt prompt,
        string functionJson,
        CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Add("Authorization", "Bearer");
        
        
        string json = JsonSerializer.Serialize(prompt, JsonOptions);
        // Parse the JSON string into a JsonNode
        JsonNode? jsonNode = JsonNode.Parse(json);
        // Cast the JsonNode to a JsonObject
        JsonObject jsonObject = (JsonObject)jsonNode!;
        
        jsonNode.AsObject().Add("tools", );
        
        //Console.WriteLine(json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;

        HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        return response.Content
            .ReadFromJsonAsync<GptResponse>(JsonOptions, cancellationToken: cancellationToken).Result!;
    }
}
using AiDevsRag.OpenAI.Request;
using System.Text.Json.Serialization;

namespace AiDevsRag.OpenAI;

[method: JsonConstructor]
public class GptMessage(
    GptMessageRole role,
    string content)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonPropertyName("role")] 
    public GptMessageRole Role { get; } = role;

    [JsonPropertyName("content")] 
    public string Content { get; } = content;

    [JsonPropertyName("tool_calls")]
    public List<ToolCall> ToolCalls { get; set; } = [];
}

public sealed class ToolCall
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("function")]
    public Function Function { get; set; } = new();
}

public sealed class Function
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("arguments")]
    public string Arguments { get; set; } = string.Empty;
}
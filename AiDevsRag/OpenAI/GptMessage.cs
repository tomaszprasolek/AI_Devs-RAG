using System.Text.Json.Serialization;

namespace AiDevsRag.OpenAI.Request;

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
    public List<ToolCall> tool_calls { get; set; }
}

public sealed class ToolCall
{
    [JsonPropertyName("id")]
    public string id { get; set; }

    [JsonPropertyName("type")]
    public string type { get; set; }

    [JsonPropertyName("function")]
    public Function function { get; set; }
}

public sealed class Function
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("arguments")]
    public string arguments { get; set; }
}
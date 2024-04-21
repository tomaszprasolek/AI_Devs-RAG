using System.Text.Json.Serialization;

namespace AiDevsRag.Helpers;

public sealed class Message
{
    [JsonConstructor]
    public Message(string role,
        string content)
    {
        Role = role;
        Content = content;
    }

    public string Role { get; set; }
    public string Content { get; set; }
    public string? Name { get; set; }
    
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
    public Function? function { get; set; }
}

public sealed class Function
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("arguments")]
    public string arguments { get; set; }
}
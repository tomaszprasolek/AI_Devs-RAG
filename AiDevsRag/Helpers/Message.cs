using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AiDevsRag.Helpers;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class Message
{
    [JsonConstructor]
    public Message(string role,
        string content)
    {
        Role = role;
        Content = content;
    }

    [JsonPropertyName("role")]
    public string Role { get; set; }
    
    [JsonPropertyName("content")]
    public string Content { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("tool_calls")]
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<ToolCall>? ToolCalls { get; set; } = [];
}


public sealed class ToolCall
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("function")]
    public Function? Function { get; set; } = new();
}

public sealed class Function
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("arguments")]
    public string Arguments { get; set; } = string.Empty;
}
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
}
namespace AiDevsRag.Helpers;

public sealed class Message
{
    public Message(string role,
        string content)
    {
        Role = role;
        Content = content;
    }

    string Role { get; set; }
    string Content { get; set; }
    string? Name { get; set; }
}
namespace AiDevsRag.Helpers;

public class Token
{
    public string Role { get; set; }
    public string Content { get; set; }

    public Token()
    {
        // Default constructor
    }

    public Token(string role, string content)
    {
        Role = role;
        Content = content;
    }
}
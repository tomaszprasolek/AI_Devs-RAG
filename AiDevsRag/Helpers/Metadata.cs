namespace AiDevsRag.Helpers;

public class Metadata
{
    public string Id { get; set; } = string.Empty;
    public string Header { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public int Tokens { get; set; }
    public string Content { get; set; } = string.Empty;
    public string[] Tags { get; set; } = [];
}
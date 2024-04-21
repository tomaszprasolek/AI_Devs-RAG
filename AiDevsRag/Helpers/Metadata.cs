namespace AiDevsRag.Helpers;

public class Metadata
{
    public string Id { get; set; }
    public string Header { get; set; }
    public string Title { get; set; }
    public string Context { get; set; }
    public string Source { get; set; }
    public int Tokens { get; set; }
    public string Content { get; set; }
    
    public string[] Tags { get; set; }
}
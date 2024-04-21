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

    public Metadata()
    {
        // Default constructor
    }

    public Metadata(string id,
        string header,
        string title,
        string context,
        string source,
        int tokens,
        string content)
    {
        Id = id;
        Header = header;
        Title = title;
        Context = context;
        Source = source;
        Tokens = tokens;
        Content = content;
    }

}
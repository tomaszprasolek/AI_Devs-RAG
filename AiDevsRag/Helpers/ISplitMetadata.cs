namespace AiDevsRag.Helpers;

public interface ISplitMetadata
{
    string Title { get; set; }
    string Header { get; set; }
    string Context { get; set; }
    string Source { get; set; }
    int Size { get; set; }
    bool Estimate { get; set; }
    string Url { get; set; }
}

public class SplitMetadata : ISplitMetadata
{
    public string Title { get; set; }
    public string Header { get; set; }
    public string Context { get; set; }
    public string Source { get; set; }
    public int Size { get; set; }
    public bool Estimate { get; set; }
    public string Url { get; set; }
}
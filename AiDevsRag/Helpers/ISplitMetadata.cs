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
namespace AiDevsRag.Helpers;

public sealed class SplitMetadata(
    string title,
    int size,
    bool estimate,
    string url,
    string? context = null)
{
    public string Title { get; } = title;
    public int Size { get; } = size;
    public bool Estimate { get; } = estimate;
    public string Url { get; } = url;
    public string? Context { get; } = context;
}
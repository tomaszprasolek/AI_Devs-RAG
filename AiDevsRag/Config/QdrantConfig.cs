namespace AiDevsRag.Config;

public sealed class QdrantConfig // TODO: move it to Qdrant folder ???
{
    public const string ConfigKey = "Qdrant";
    
    public string? BaseUrl { get; set; }
    public string? CollectionName { get; set; }
    public bool ImportDocuments { get; set; } = false;
}
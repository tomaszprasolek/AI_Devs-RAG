namespace AiDevsRag.Config;

public sealed class QdrantConfig // TODO: move it to Qdrant folder ???
{
    public const string ConfigKey = "Qdrant";

    public string BaseUrl { get; set; } = string.Empty;
    public string CollectionName { get; set; } = string.Empty;
    public bool ImportDocuments { get; set; } = false;
    public bool GenerateTags { get; set; } = false;
}
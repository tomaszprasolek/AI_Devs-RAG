namespace AiDevsRag.Services;

public sealed class RerankCheck(string documentId, int rank)
{
    public string DocumentId { get; } = documentId;
    public int Rank { get; } = rank;
}
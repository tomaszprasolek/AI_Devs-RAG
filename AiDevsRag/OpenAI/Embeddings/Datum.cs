namespace AiDevsRag.OpenAI.Embeddings;

public sealed class Datum
{
    public string Object { get; set; } = string.Empty;
    public int Index { get; set; }
    public List<double> Embedding { get; set; } = [];
}
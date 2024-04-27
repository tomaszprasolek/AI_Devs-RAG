using System.Diagnostics.CodeAnalysis;

namespace AiDevsRag.OpenAI.Embeddings;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public sealed class Embedding
{
    public string Object { get; set; } = string.Empty;
    public List<Datum> Data { get; set; } = [];
    public string Model { get; set; } = string.Empty;
    public Usage Usage { get; set; } = new();
}
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant.Embeddings;


[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class QdrantPoints
{
    [JsonPropertyName("points")]
    public List<Point> Points { get; set; } = new();
}
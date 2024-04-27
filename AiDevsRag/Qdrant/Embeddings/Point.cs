using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant.Embeddings;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class Point
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("vector")]
    public List<double> Vector { get; set; } = [];
    
    [JsonPropertyName("payload")]
    public Payload Payload { get; set; } = new();
}
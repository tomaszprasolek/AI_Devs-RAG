using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant.Embeddings;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class Payload
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("DocumentName")]
    public string DocumentName { get; set; } = string.Empty;
}
using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant.Search;

public sealed class QdrantSearchRequest
{
    [JsonPropertyName("vector")]
    public List<double> Vector { get; set; } = [];

    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 1;

    [JsonPropertyName("with_payload")]
    public bool WithPayload { get; set; } = true;
}
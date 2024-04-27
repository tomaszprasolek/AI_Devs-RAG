using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant.Search;

public sealed class QdrantSearchResponse
{
    [JsonPropertyName("result")]
    public List<Result> Result { get; set; } = [];

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public double Time { get; set; }
}
using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant;

[method: JsonConstructor]
public class Result(
    string status,
    string optimizerStatus,
    int vectorsCount,
    int indexedVectorsCount,
    int pointsCount,
    int segmentsCount)
{
    [JsonPropertyName("status")]
    public string Status { get; } = status;

    [JsonPropertyName("optimizer_status")]
    public string OptimizerStatus { get; } = optimizerStatus;

    [JsonPropertyName("vectors_count")]
    public int VectorsCount { get; } = vectorsCount;

    [JsonPropertyName("indexed_vectors_count")]
    public int IndexedVectorsCount { get; } = indexedVectorsCount;

    [JsonPropertyName("points_count")]
    public int PointsCount { get; } = pointsCount;

    [JsonPropertyName("segments_count")]
    public int SegmentsCount { get; } = segmentsCount;
}
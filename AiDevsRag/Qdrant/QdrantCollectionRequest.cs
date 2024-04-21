using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant;

public class QdrantCollectionRequest(Vectors vectors)
{
    [JsonPropertyName("vectors")]
    public Vectors Vectors { get; } = vectors;
}
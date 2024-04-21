using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant;

public class QdrantCollection
{
    public QdrantCollection(Vectors vectors)
    {
        Vectors = vectors;
    }

    [JsonPropertyName("vectors")]
    public Vectors Vectors { get; }
}
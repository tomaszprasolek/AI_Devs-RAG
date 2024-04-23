using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant.Embeddings;


public sealed class QdrantPoints
{
    [JsonPropertyName("points")]
    public List<Point> Points { get; set; }
}

public sealed class Payload
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
    
    [JsonPropertyName("DocumentName")]
    public string DocumentName { get; set; }
}

public sealed class Point
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("vector")]
    public List<double> Vector { get; set; }
    
    [JsonPropertyName("payload")]
    public Payload Payload { get; set; }
}

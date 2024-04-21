using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant;

public class Vectors
{
    [JsonPropertyName("size")] 
    public int Size { get; } = 1536;

    [JsonPropertyName("distance")] 
    public string Distance { get; } = "Cosine";
}
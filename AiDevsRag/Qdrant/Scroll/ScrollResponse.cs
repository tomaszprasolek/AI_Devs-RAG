using System.Text.Json.Serialization;
// ReSharper disable CollectionNeverUpdated.Global

namespace AiDevsRag.Qdrant.Scroll;

public class ScrollResponse
{
    [JsonPropertyName("result")]
    public Result Result { get; set; } = new();
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("time")]
    public double Time { get; set; }
}


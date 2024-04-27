using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant.Scroll;

public class Result
{
    [JsonPropertyName("points")]
    public List<object> Points { get; set; } = [];

    [JsonPropertyName("next_page_offset")]
    public object NextPageOffset { get; set; } = new();
}
using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant;

[method: JsonConstructor]
public class QdrantCollectionResponse(
    Result result,
    string status,
    double time)
{
    [JsonPropertyName("result")]
    public Result Result { get; } = result;

    [JsonPropertyName("status")]
    public string Status { get; } = status;

    [JsonPropertyName("time")]
    public double Time { get; } = time;
}
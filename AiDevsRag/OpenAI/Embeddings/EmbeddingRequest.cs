using System.Text.Json.Serialization;

namespace AiDevsRag.OpenAI.Embeddings;

[method: JsonConstructor]
public class EmbeddingRequest(
    string input,
    string model = "text-embedding-ada-002")
{
    [JsonPropertyName("input")]
    public string Input { get; } = input;
    
    [JsonPropertyName("model")]
    public string Model { get; } = model;
}
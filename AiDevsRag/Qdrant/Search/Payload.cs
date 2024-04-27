using AiDevsRag.Helpers;
using AiDevsRag.OpenAI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant.Search;

public sealed class Payload
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    public Metadata? GetMetadata()
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return null;
        }

        return JsonSerializer.Deserialize<Metadata>(Text, OpenAiService.JsonOptions);
    }
}
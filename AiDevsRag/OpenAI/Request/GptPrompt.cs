using System.Text.Json.Serialization;

namespace AiDevsRag.OpenAI.Request;

public sealed class GptPrompt
{
    [JsonConstructor]
    public GptPrompt(string model = "gpt-3.5-turbo-0125")
    {
        Model = model;
        Messages = new List<GptMessage>();
    }

    /// <summary>
    /// Gets the GPT model. Default: gpt-3.5-turbo-0125.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; } = "gpt-3.5-turbo-0125";

    [JsonPropertyName("messages")] 
    public List<GptMessage> Messages { get; }

    [JsonPropertyName("tool_choice")] 
    public string? ToolChoice { get; set; }

    [JsonPropertyName("temperature")] 
    public float Temperature { get; set; } = 1;
    
    [JsonPropertyName("maxConcurrency")]
    public int? MaxConcurrency { get; set; }

    public void AddMessage(GptMessage message)
    {
        Messages.Add(message);
    }
}
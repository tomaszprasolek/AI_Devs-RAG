namespace AiDevsRag.OpenAI.Response;

public sealed class GptResponse
{
    public string Id { get; set; } = string.Empty;
    public string Object { get; set; } = string.Empty;
    public int Created { get; set; }
    public string Model { get; set; } = string.Empty;
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<Choice> Choices { get; set; } = [];
    public Usage Usage { get; set; } = new();
    public object SystemFingerprint { get; set; } = new();
}
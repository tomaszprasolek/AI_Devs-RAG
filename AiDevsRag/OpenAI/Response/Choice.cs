using AiDevsRag.Helpers;

namespace AiDevsRag.OpenAI.Response;

public sealed class Choice
{
    public int Index { get; set; }
    public Message Message { get; set; }
    public object Logprobs { get; set; }
    public string FinishReason { get; set; }
}
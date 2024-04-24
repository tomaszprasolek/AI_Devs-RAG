using AiDevsRag.OpenAI.Embeddings;
using AiDevsRag.OpenAI.Request;
using AiDevsRag.OpenAI.Response;

namespace AiDevsRag.OpenAI;

public interface IOpenAiService
{
    Task<GptResponse> ChatAsync(GptPrompt prompt,
        CancellationToken cancellationToken);

    Task<GptResponse?> ChatWithFunctionAsync(GptPrompt prompt,
        string functionJson,
        CancellationToken cancellationToken);

    Task<Embedding?> GenerateEmbeddingsAsync(EmbeddingRequest embeddingRequest,
        CancellationToken cancellationToken);
}
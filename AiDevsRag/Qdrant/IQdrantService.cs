using AiDevsRag.Qdrant.Embeddings;
using AiDevsRag.Qdrant.Search;

namespace AiDevsRag.Qdrant;

public interface IQdrantService
{
    Task CreateCollectionAsync();
    Task<bool> CheckIfCollectionExistsAsync();
    Task<QdrantCollectionResponse?> GetCollectionInfoAsync();

    Task UpsertPointsAsync(QdrantPoints points,
        CancellationToken cancellationToken);

    Task<QdrantSearchResponse?> SearchAsync(QdrantSearchRequest searchRequest,
        CancellationToken cancellationToken);
}
using AiDevsRag.Qdrant.Embeddings;
using AiDevsRag.Qdrant.Search;

namespace AiDevsRag.Qdrant;

public interface IQdrantService
{
    Task CreateCollectionAsync(string collectionName);
    Task<bool> CheckIfCollectionExistsAsync(string collectionName);
    Task<QdrantCollectionResponse?> GetCollectionInfoAsync(string collectionName);

    Task UpsertPointsAsync(string collectionName,
        QdrantPoints points,
        CancellationToken cancellationToken);

    Task<QdrantSearchResponse?> SearchAsync(string collectionName,
        QdrantSearchRequest searchRequest,
        CancellationToken cancellationToken);
}
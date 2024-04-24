using AiDevsRag.Qdrant.Embeddings;
using AiDevsRag.Qdrant.Search;

namespace AiDevsRag.Qdrant;

public interface IQdrantService
{
    Task CreateCollectionAsync(CancellationToken cancellationToken);
    Task<bool> CheckIfCollectionExistsAsync(CancellationToken cancellationToken);
    Task<QdrantCollectionResponse?> GetCollectionInfoAsync(CancellationToken cancellationToken);

    Task UpsertPointsAsync(QdrantPoints points,
        CancellationToken cancellationToken);

    Task<QdrantSearchResponse?> SearchAsync(QdrantSearchRequest searchRequest,
        CancellationToken cancellationToken);

    Task<bool> CheckIfDocumentExistAsync(string documentName,
        CancellationToken cancellationToken);
}
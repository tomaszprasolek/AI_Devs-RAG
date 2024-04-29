using AiDevsRag.Qdrant.Search;

namespace AiDevsRag.Services;

public interface IReRankService
{
    Task<List<Result>> RerankAsync(string query,
        QdrantSearchResponse search,
        CancellationToken cancellationToken);
}
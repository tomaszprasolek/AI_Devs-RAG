using AiDevsRag.Helpers;
using AiDevsRag.OpenAI;
using AiDevsRag.OpenAI.Request;
using AiDevsRag.Qdrant.Search;

namespace AiDevsRag.Services;

public sealed class ReRankService : IReRankService
{
    private readonly IOpenAiService _openAiService;

    private readonly string _systemPrompt = """"
                                            Check if the following document is relevant to this user query: "##query##" and the lesson of the course (if its mentioned by the user) and may be helpful to answer the question / query.
                                            Return 0 if not relevant, 1 if relevant.

                                            Warning:
                                            - You're forced to return 0 or 1 and forbidden to return anything else under any circumstances.
                                            - Pay attention to the keywords from the query, mentioned links etc.

                                            Additional info:
                                            - Document title: ##title##
                                            - Document context (may be helpful): ##header##

                                            Document content: ## ##content## ##

                                            Query:
                                            """";

    
    public ReRankService(IOpenAiService openAiService)
    {
        _openAiService = openAiService;
    }
    
    public async Task<List<Result>> RerankAsync(string query,
        QdrantSearchResponse search,
        CancellationToken cancellationToken)
    {
        // Order by Score - descending
        List<Result> documents = search
            .Result
            .OrderByDescending(x => x.Score)
            .ToList();

        // Prepare Task for async job
        List<Task<RerankCheck>> reRankTasks = documents
            .Select(document => document.Payload.GetMetadata())
            .OfType<Metadata>()
            .Select(metadata => RankDocumentAsync(metadata, query, _systemPrompt, cancellationToken))
            .ToList();

        List<RerankCheck> rerankChecks = (await Task.WhenAll(reRankTasks)).ToList();
        
        var results = new List<Result>();
        
        foreach (Result document in documents)
        {
            Metadata? metadata = document.Payload.GetMetadata();  // TODO: refactor this
            if (metadata is null) continue;

            string id = document.Payload.GetMetadata()!.Id;

            bool isRelevant = rerankChecks.Any(x => x.DocumentId == id && x.Rank == 1);
            if (isRelevant)
                results.Add(document);
        }

        return FilterResults(results);
    }

    private async Task<RerankCheck> RankDocumentAsync(Metadata metadata,
        string query,
        string systemPrompt,
        CancellationToken cancellationToken)
    {
        systemPrompt = systemPrompt
            .Replace("##query##", query)
            .Replace("##title##", metadata.Title)
            .Replace("##header##", string.IsNullOrWhiteSpace(metadata.Header) ? "n/a" : metadata.Header)
            .Replace("##content##", metadata.Content);

        string userMessage = $"{query}### Is relevant (0 or 1)";

        GptPrompt prompt = new GptPrompt("gpt-3.5-turbo-16k")
        {
            Temperature = 0
        };
        prompt.AddMessage(new GptMessage(GptMessageRole.system, systemPrompt));
        prompt.AddMessage(new GptMessage(GptMessageRole.user, userMessage));
        var gptResponse = await _openAiService.ChatAsync(prompt, cancellationToken);

        return new RerankCheck(metadata.Id, Convert.ToInt32(gptResponse.Choices[0].Message.Content));
    }

    private static List<Result> FilterResults(List<Result> reranked)
    {
        var results = new List<Result>(reranked.Count);
        int limit = 5500;
        int current = 0;

        foreach (Result rerank in reranked)
        {
            Metadata? metadata = rerank.Payload.GetMetadata();
            if (metadata is null) continue;
            
            int tokens = metadata.Tokens;
            if (current + tokens < limit)
            {
                current += tokens;
                results.Add(rerank);
            }
        }

        return results;
    }
}
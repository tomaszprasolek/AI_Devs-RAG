using AiDevsRag.Config;
using AiDevsRag.Helpers;
using AiDevsRag.OpenAI;
using AiDevsRag.OpenAI.Common;
using AiDevsRag.OpenAI.Embeddings;
using AiDevsRag.OpenAI.Request;
using AiDevsRag.OpenAI.Response;
using AiDevsRag.Qdrant;
using AiDevsRag.Qdrant.Embeddings;
using AiDevsRag.Qdrant.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Result = AiDevsRag.Qdrant.Search.Result;

namespace AiDevsRag;

public sealed class ApplicationLogic
{
    private readonly IQdrantService _qdrantDatabase;
    private readonly IOpenAiService _openAiService;
    private readonly ILogger<QdrantConfig> _logger;

    private readonly bool _importDocuments;
    private readonly string _collectionName;
    private readonly bool _generateTags;

    public ApplicationLogic(IQdrantService qdrantDatabase,
        IOptions<QdrantConfig> qdrantConfig,
        IOpenAiService openAiService,
        ILogger<QdrantConfig> logger)

    {
        _qdrantDatabase = qdrantDatabase;
        _openAiService = openAiService;
        _logger = logger;

        _importDocuments = qdrantConfig.Value.ImportDocuments;
        _collectionName = qdrantConfig.Value.CollectionName;
        _generateTags = qdrantConfig.Value.GenerateTags;
    }

    public async Task LoadMemoryAsync(CancellationToken cancellationToken)
    {
        await CreateQdrantCollectionIfNotExist(cancellationToken);

        QdrantCollectionResponse? collection = await _qdrantDatabase.GetCollectionInfoAsync(cancellationToken);
        if (collection is null || collection.Result.PointsCount == 0 || _importDocuments)
        {
            // Generate documents to be ready to load them to Qdrant
            _logger.LogInformation("Start loading memories to Qdrant database...");
            
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Memories");

            foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.md"))
            {
                // Check if file is already imported
                bool docExists =
                    await _qdrantDatabase.CheckIfDocumentExistAsync(Path.GetFileNameWithoutExtension(filePath),
                        cancellationToken);
                if (docExists)
                    continue;
                
                List<Document> fileMemories = await LoadMemoriesFromFileAsync(filePath, cancellationToken);

                // Load document to vector database
                foreach (Document document in fileMemories)
                {
                    var embeddings =
                        await _openAiService.GenerateEmbeddingsAsync(new EmbeddingRequest(document.PageContent),
                            cancellationToken);

                    if (embeddings is null)
                    {
                        continue;
                    }

                    QdrantPoints points = new QdrantPoints
                    {
                        Points =
                        [
                            new Point
                            {
                                Id = document.Metadata.Id,
                                Payload = new Qdrant.Embeddings.Payload
                                {
                                    DocumentName = document.Metadata.Title,
                                    Text = JsonSerializer.Serialize(document.Metadata)
                                },
                                Vector = embeddings.Data[0].Embedding
                            }
                        ]
                    };

                    // Insert to vector database
                    await _qdrantDatabase.UpsertPointsAsync(points, cancellationToken);
                }
            }

        }
    }

    private async Task CreateQdrantCollectionIfNotExist(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Check if collection '{CollectionName}' exists...", _collectionName);

        if (!await _qdrantDatabase.CheckIfCollectionExistsAsync(cancellationToken))
        {
            _logger.LogInformation("Collection {CollectionName} not exist", _collectionName);
            _logger.LogInformation("Creating collection: {CollectionName}", _collectionName);

            await _qdrantDatabase.CreateCollectionAsync(cancellationToken);
        }

        _logger.LogInformation("Collection {CollectionName} already exists", _collectionName);
    }

    public async Task<QdrantSearchResponse> SearchAsync(string query, CancellationToken cancellationToken)
    {
        Embedding? queryEmbedding = await _openAiService.GenerateEmbeddingsAsync(new EmbeddingRequest(query), cancellationToken);
        if (queryEmbedding is null)
        {
            throw new Exception("Embedding is null");
        }
        
        QdrantSearchResponse? result = await _qdrantDatabase.SearchAsync(new QdrantSearchRequest
        {
            Limit = 5,
            Vector = queryEmbedding.Data[0].Embedding
        }, cancellationToken);

        if (result is null)
        {
            throw new Exception("Qdrant search result is null");
        }
        return result;
    }

    public async Task AskLlmAsync(string question, 
        List<Result> qdrantSearchResults, 
        CancellationToken cancellationToken)
    {
        var gptPrompt = new GptPrompt
        {
            Temperature = 0.5f
        };
        gptPrompt.AddMessage(new GptMessage(GptMessageRole.system, Prompts.GetSystemPrompt(qdrantSearchResults)));
        gptPrompt.AddMessage(new GptMessage(GptMessageRole.user, question));
        
        GptResponse response = await _openAiService.ChatAsync(gptPrompt, cancellationToken);
        
        Console.WriteLine("Answer:");
        Console.WriteLine(response.Choices[0].Message.Content);
    }

    private async Task<List<Document>> LoadMemoriesFromFileAsync(string filePath,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processing file: {Path.GetFileNameWithoutExtension(filePath)}");

        string content = await File.ReadAllTextAsync(filePath, cancellationToken);
        string title = Path.GetFileNameWithoutExtension(filePath);

        List<Document> documents = DocumentsHelpers.Split(content, new SplitMetadata
        {
            Title = title,
            Size = 2500,
            Estimate = true,
            Url = "https://bravecourses.circle.so/c/lekcje-programu-ai2r-fc066c/"
        });


        if (_generateTags)
        {
            foreach (Document document in documents)
            {
                document.Metadata.Tags = await GenerateTagsAsync(document, cancellationToken);
            }
        }

        return documents;
    }

    private async Task<string[]> GenerateTagsAsync(Document document,
        CancellationToken cancellationToken)
    {
        string functionJson =
            await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "OpenAI", "FunctionCalling",
                "generate_tags.json"), cancellationToken);

        var prompt = new GptPrompt("gpt-4-0613")
        {
            Temperature = 0
        };
        
        string title = document.Metadata.Title;
        string header = document.Metadata.Header;

        string systemPrompt = "Generate tags for the following document.\n\r" +
                              "Additional info: \n\r" +
                              $"- Document title: {title}\n\r" +
                              $"- Document context (may be helpful): {(!string.IsNullOrWhiteSpace(header) ? header : "n/a")}\n\r";
        prompt.AddMessage(new GptMessage(GptMessageRole.system, systemPrompt));
        prompt.AddMessage(new GptMessage(GptMessageRole.user, document.PageContent));

        GptResponse? gptResponse = await _openAiService.ChatWithFunctionAsync(prompt, functionJson, cancellationToken);

        return ParseFunctionCall(gptResponse);
    }

    private static string[] ParseFunctionCall(GptResponse response)
    {
        string json;
        
        if (response.Choices[0].Message.tool_calls is null)
        {
            try
            {
                JsonDocument jsonDocument = JsonDocument.Parse(response.Choices[0].Message.Content);
                json = JsonSerializer.Serialize(jsonDocument.RootElement);
            }
            catch
            {
                string[] tagsTemp = response.Choices[0].Message.Content
                    .Split('\n')
                    .Select(x => Regex.Replace(x
                        .ToLower()
                        .Replace("-", "")
                        .Trim(), 
                        "[-\\s]+", "_"))
                    .ToArray();

                var tagsContainer = new TagsContainer()
                {
                    Tags = tagsTemp
                };
                
                json = JsonSerializer.Serialize(tagsContainer, OpenAiService.JsonOptions);
            }
        }
        else
        {
            json = response.Choices[0].Message.tool_calls[0].function!.arguments;
        }

        // Deserialize the JSON string into an instance of TagsContainer
        TagsContainer? container = JsonSerializer.Deserialize<TagsContainer>(json);
        if (container is null)
            return [];
        
        string[] tags = container.Tags
            .Select(x => Regex.Replace(x.ToLower(),"[-\\s]+", "_"))
            .ToArray();
        return tags;
    }
    
                
    // Define a class that matches the JSON structure
    private sealed class TagsContainer
    {
        [JsonPropertyName("tags")]
        public string[] Tags { get; set; } = [];
    }
}
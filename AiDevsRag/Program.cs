using AiDevsRag.Helpers;
using AiDevsRag.OpenAI;
using AiDevsRag.OpenAI.Request;
using AiDevsRag.Qdrant;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string openAiApiKey = configuration["OPENAI_API_KEY"] ?? throw new InvalidOperationException("OPENAI_API_KEY");

Console.WriteLine("Starting the app...");

CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
await LoadMemoryAsync(cancellationTokenSource.Token);
return;

// -----------------------------
// END OF THE PROGRAM
// -----------------------------

async Task LoadMemoryAsync(CancellationToken cancellationToken) // TODO: move it to separete class
{
    string collectionName = "ai_devs";
    
    var service = new QdrantService();
    
    Console.WriteLine($"Check if collection '{collectionName}' exists...");
    
    if (!await service.CheckIfCollectionExistsAsync(collectionName))
    {
        Console.WriteLine($"Collection {collectionName} not exist");
        Console.WriteLine($"Creating collection: {collectionName}");
        
        await service.CreateCollectionAsync(collectionName);
    }
    Console.WriteLine($"Collection {collectionName} already exists");

    QdrantCollectionResponse? collection = await service.GetCollectionInfoAsync(collectionName);
    if (collection is null || collection.Result.PointsCount == 0)
    {
        // Load document to vector database
        Console.WriteLine("Start loading memories to Qdrant database...");
        await LoadMemoriesAsync(cancellationToken);
    }
}

async Task LoadMemoriesAsync(CancellationToken cancellationToken)
{
    string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Memories");
    
    foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*.md"))
    {
        string content = File.ReadAllText(filePath);
        var title = Path.GetFileNameWithoutExtension(filePath);

        List<Document> documents = DocumentsHelpers.Split(content, new SplitMetadata
        {
            Title = title,
            Size = 2500,
            Estimate = true,
            Url = "https://bravecourses.circle.so/c/lekcje-programu-ai2r-fc066c/"
        });
        
        foreach (Document document in documents)
        {
            await GenerateTagsAsync(document, new EnrichMetadata(document.Metadata.Title, document.Metadata.Header),
                cancellationToken);
        }
    }
}

async Task GenerateTagsAsync(Document document,
    EnrichMetadata config,
    CancellationToken cancellationToken)
{
    string functionJson =
        File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "OpenAI", "FunctionCalling",
            "generate_tags.json"));

    var prompt = new GptPrompt("gpt-4-0613")
    {
        Temperature = 0,
        MaxConcurrency = 5
    };

    string systemPrompt = "Generate tags for the following document.\n\r" +
                          "Additional info: \n\r" +
                          $"- Document title: {config.Title}\n\r" +
                          $"- Document context (may be helpful): {config.Header ?? "n/a"}\n\r";
    prompt.AddMessage(new GptMessage(GptMessageRole.System, systemPrompt));
    prompt.AddMessage(new GptMessage(GptMessageRole.User, document.PageContent));

    var openAi = new OpenAiService();
    var gptResponse = await openAi.ChatWithFunctionAsync(prompt, functionJson, cancellationToken);
}

public sealed class EnrichMetadata
{
    public EnrichMetadata(string title,
        string url)
    {
        Title = title;
        Url = url;
    }

    public string Title { get; }
    public string? Header { get; init; }
    public string? Context { get; init; }
    public string? Source { get; init; }
    public string Url { get; }
}
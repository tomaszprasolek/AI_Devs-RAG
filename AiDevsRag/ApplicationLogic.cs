using AiDevsRag.Helpers;
using AiDevsRag.OpenAI;
using AiDevsRag.OpenAI.Request;
using AiDevsRag.OpenAI.Response;
using AiDevsRag.Qdrant;

namespace AiDevsRag;

public sealed class ApplicationLogic(
    IQdrantService qdrantDatabase,
    IOpenAiService openAiService)
{
    public async Task LoadMemoryAsync(CancellationToken cancellationToken) // TODO: move it to separete class
    {
        string collectionName = "ai_devs";

        Console.WriteLine($"Check if collection '{collectionName}' exists...");

        if (!await qdrantDatabase.CheckIfCollectionExistsAsync(collectionName))
        {
            Console.WriteLine($"Collection {collectionName} not exist");
            Console.WriteLine($"Creating collection: {collectionName}");

            await qdrantDatabase.CreateCollectionAsync(collectionName);
        }

        Console.WriteLine($"Collection {collectionName} already exists");

        QdrantCollectionResponse? collection = await qdrantDatabase.GetCollectionInfoAsync(collectionName);
        if (collection is null || collection.Result.PointsCount == 0)
        {
            // Load document to vector database
            Console.WriteLine("Start loading memories to Qdrant database...");
            await LoadMemoriesAsync(cancellationToken);
        }
    }

    private async Task LoadMemoriesAsync(CancellationToken cancellationToken)
    {
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Memories");

        foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.md"))
        {
            string content = await File.ReadAllTextAsync(filePath, cancellationToken);
            string title = Path.GetFileNameWithoutExtension(filePath);

            List<Document> documents = DocumentsHelpers.Split(content, new SplitMetadata
            {
                Title = title,
                Size = 2500,
                Estimate = true,
                Url = "https://bravecourses.circle.so/c/lekcje-programu-ai2r-fc066c/"
            });

            foreach (Document document in documents)
                await GenerateTagsAsync(document, new EnrichMetadata(document.Metadata.Title, document.Metadata.Header),
                    cancellationToken);
        }
    }

    private async Task GenerateTagsAsync(Document document,
        EnrichMetadata config,
        CancellationToken cancellationToken)
    {
        string functionJson =
            await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "OpenAI", "FunctionCalling",
                "generate_tags.json"), cancellationToken);

        var prompt = new GptPrompt("gpt-4-0613")
        {
            Temperature = 0
            //MaxConcurrency = 5
        };

        string systemPrompt = "Generate tags for the following document.\n\r" +
                              "Additional info: \n\r" +
                              $"- Document title: {config.Title}\n\r" +
                              $"- Document context (may be helpful): {config.Header ?? "n/a"}\n\r";
        prompt.AddMessage(new GptMessage(GptMessageRole.system, systemPrompt));
        prompt.AddMessage(new GptMessage(GptMessageRole.user, document.PageContent));

        GptResponse? gptResponse = await openAiService.ChatWithFunctionAsync(prompt, functionJson, cancellationToken);
    }
}
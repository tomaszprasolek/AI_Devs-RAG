using AiDevsRag.Helpers;
using AiDevsRag.Qdrant;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var openAiApiKey = configuration["OPENAI_API_KEY"] ?? throw new InvalidOperationException("OPENAI_API_KEY");

Console.WriteLine("Starting the app...");

await LoadMemoryAsync();
return;

// -----------------------------
// END OF THE PROGRAM
// -----------------------------

async Task LoadMemoryAsync() // TODO: move it to separete class
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
        await LoadMemories();
    }
}

async Task LoadMemories()
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
    }
}
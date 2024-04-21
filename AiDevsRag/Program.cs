using AiDevsRag.Qdrant;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var openAiApiKey = configuration["OPENAI_API_KEY"] ?? throw new InvalidOperationException("OPENAI_API_KEY");

Console.WriteLine("Starting the app...");

await LoadMemoryAsync();

async Task LoadMemoryAsync() // TODO: move it to separete class
{
    string collectionName = "ai_devs";
    
    var service = new QdrantService();
    
    Console.WriteLine($"Check if collection '{collectionName}' exists...");
    if (!await service.CheckIfCollectionExists(collectionName))
    {
        Console.WriteLine($"Collection {collectionName} not exist");
        Console.WriteLine($"Creating collection: {collectionName}");
        await service.CreateCollection(collectionName);
    }
    Console.WriteLine($"Collection {collectionName} already exists");
}
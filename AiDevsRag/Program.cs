using AiDevsRag;
using AiDevsRag.Config;
using AiDevsRag.OpenAI;
using AiDevsRag.Qdrant;
using AiDevsRag.Qdrant.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>()
    .Build();

var services = new ServiceCollection();
services.AddOptions<OpenAiConfig>().Bind(configuration.GetSection(OpenAiConfig.ConfigKey));
services.AddOptions<QdrantConfig>().Bind(configuration.GetSection(QdrantConfig.ConfigKey));
services.AddSingleton<IOpenAiService, OpenAiService>();
services.AddSingleton<IQdrantService, QdrantService>();
services.AddSingleton<ApplicationLogic>();

var serviceProvider = services.BuildServiceProvider();

// -------------
// START APP
// -------------
Console.WriteLine("Starting the app...");
CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
var app = serviceProvider.GetRequiredService<ApplicationLogic>();
await app.LoadMemoryAsync(cancellationTokenSource.Token);


while (true)
{
    // User question
    Console.WriteLine("Your question about AI_Devs course:");
    string? question = Console.ReadLine();
    if (question is null || 
        question.Equals("exit", StringComparison.CurrentCultureIgnoreCase) || 
        question.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
    {
        Console.WriteLine("No question");
        Environment.Exit(0);
    }
    
    // Search the answer
    QdrantSearchResponse searchResult = await app.SearchAsync(question, "ai_devs", cancellationTokenSource.Token);
    await app.AskLlmAsync(question, searchResult.Result, cancellationTokenSource.Token);
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
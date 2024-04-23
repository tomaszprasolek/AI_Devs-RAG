using AiDevsRag;
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
services.AddSingleton<IOpenAiService, OpenAiService>();
services.AddSingleton<IQdrantService, QdrantService>();
services.AddSingleton<ApplicationLogic>();

var serviceProvider = services.BuildServiceProvider();

string question = "In which lesson was about rerank?";
Console.WriteLine($"Question: {question}");

Console.WriteLine("Starting the app...");

CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

var app = serviceProvider.GetRequiredService<ApplicationLogic>();

await app.LoadMemoryAsync(cancellationTokenSource.Token);
QdrantSearchResponse searchResult = await app.SearchAsync(question, "ai_devs", cancellationTokenSource.Token);
await app.AskLlmAsync(question, searchResult.Result, cancellationTokenSource.Token);


public sealed class OpenAiConfig // TODO: move to separate file
{
    public const string ConfigKey = "OpenAi";
    public string ApiKey { get; set; }
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
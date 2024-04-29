using AiDevsRag;
using AiDevsRag.Config;
using AiDevsRag.OpenAI;
using AiDevsRag.Qdrant;
using AiDevsRag.Qdrant.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>()
    .Build();

ServiceCollection services = new ServiceCollection();

services.AddLogging(loggerBuilder =>
{
    loggerBuilder.ClearProviders();
    loggerBuilder.AddConfiguration(configuration.GetSection("Logging"));
    loggerBuilder.AddConsole();
});

services.AddOptions<QdrantConfig>().Bind(configuration.GetSection(QdrantConfig.ConfigKey));

// Register HTTP clients
services.AddHttpClient<IQdrantService, QdrantService>(httpClient =>
{
    string? baseUrl = configuration.GetValue<string>($"{QdrantConfig.ConfigKey}:{nameof(QdrantConfig.BaseUrl)}");
    httpClient.BaseAddress = new Uri(baseUrl!);
});

services.AddHttpClient<IOpenAiService, OpenAiService>(httpClient =>
{
    string configKey = "OpenAi";
    string baseUrl = configuration.GetValue<string>($"{configKey}:BaseUrl")!;
    string apiKey = configuration.GetValue<string>($"{configKey}:ApiKey")!;
    
    httpClient.BaseAddress = new Uri(baseUrl);
    httpClient.DefaultRequestHeaders.Clear();
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});
   
services.AddSingleton<ApplicationLogic>();

var serviceProvider = services.BuildServiceProvider();

// -------------
// START APP
// -------------
var logger = serviceProvider.GetService<ILogger<Program>>()!;

logger.LogInformation("Starting the app...");
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
        logger.LogInformation("No question. Close the application.");
        Environment.Exit(0);
    }

    var sw = new Stopwatch();
    sw.Start();
    
    // Search the answer
    QdrantSearchResponse searchResult = await app.SearchAsync(question, cancellationTokenSource.Token);
    // Re-rank
    var reRankedResult = await app.RerankAsync(question, searchResult, cancellationTokenSource.Token);
    // Final answer
    await app.AskLlmAsync(question, reRankedResult, cancellationTokenSource.Token);
    
    sw.Stop();
    Console.WriteLine($"Answer generated in: {sw.ElapsedMilliseconds}ms");
}
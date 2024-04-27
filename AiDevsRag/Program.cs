using AiDevsRag;
using AiDevsRag.Config;
using AiDevsRag.OpenAI;
using AiDevsRag.Qdrant;
using AiDevsRag.Qdrant.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>()
    .Build();

var services = new ServiceCollection();

services.AddLogging(loggerBuilder =>
{
    loggerBuilder.ClearProviders();
    loggerBuilder.AddConfiguration(configuration.GetSection("Logging"));
    loggerBuilder.AddConsole();
});

services.AddOptions<OpenAiConfig>().Bind(configuration.GetSection(OpenAiConfig.ConfigKey));
services.AddOptions<QdrantConfig>().Bind(configuration.GetSection(QdrantConfig.ConfigKey));
services.AddSingleton<IOpenAiService, OpenAiService>();
services.AddSingleton<IQdrantService, QdrantService>();
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
    
    // Search the answer
    QdrantSearchResponse searchResult = await app.SearchAsync(question, cancellationTokenSource.Token);
    await app.AskLlmAsync(question, searchResult.Result, cancellationTokenSource.Token);
}
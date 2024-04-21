using System.Reflection;
using System.Text.RegularExpressions;
using TiktokenSharp;

namespace AiDevsRag.Helpers;

public class DocumentsHelpers
{
    public static List<Document> Split(string text, ISplitMetadata config)
    {
        List<Document> documents = new List<Document>();
        string document = "";

        // split by header or double new line
        string[] chunks = text.Contains("#") ? Regex.Split(text, @"\n(?=\#+)") : text.Split("\n\n");
        string topic = "";

        foreach (string chunk in chunks)
        {
            if (string.IsNullOrWhiteSpace(chunk))
                continue;

            string header = Regex.Match(chunk, @"^\#.*$", RegexOptions.Multiline)?.Value ?? "n/a";
            topic = Regex.Match(chunk, @"^## [^#].*$", RegexOptions.Multiline)?.Value ?? topic;
            string uuid = Guid.NewGuid().ToString();

            int chunkTokens = config.Estimate
                ? CountTokens(new List<Message> {new("human", document + chunk)}, "gpt-4-0613")
                : (document + chunk).Length;

            if (chunkTokens > config.Size)
            {
                string subChunk = "";
                string[] sentences = Regex.Split(chunk, @"(?<=[.?!])\s+(?=[A-Z])");
                int tokenCount = 0;
                int sentenceIndex = 0;
                while (tokenCount <= config.Size && sentenceIndex < sentences.Length)
                {
                    string sentence = sentences[sentenceIndex];
                    int sentenceTokens = config.Estimate
                        ? CountTokens(new List<Message> {new("human", sentence)}, "gpt-4-0613")
                        : sentence.Length;
                    if (tokenCount + sentenceTokens > config.Size)
                    {
                        documents.Add(new Document
                        {
                            PageContent = subChunk,
                            Metadata = new Metadata
                            {
                                Id = uuid,
                                Header = header,
                                Title = config.Title,
                                Context = config.Context ?? topic,
                                Source =
                                    $"{config.Url ?? ""}{config.Title.ToLower().Replace(" — ", "-").Replace(" ", "-")}",
                                Tokens = tokenCount,
                                Content = subChunk
                            }
                        });
                        subChunk = "";
                    }

                    tokenCount += sentenceTokens;
                    subChunk += sentence;
                    sentenceIndex++;
                }
            }
            else
            {
                if (chunkTokens <= 50)
                    continue;

                documents.Add(new Document
                {
                    PageContent = chunk.Trim(),
                    Metadata = new Metadata
                    {
                        Id = uuid,
                        Header = header,
                        Title = config.Title,
                        Context = config.Context ?? topic,
                        Source = $"{config.Url ?? ""}{config.Title.ToLower().Replace(" — ", "-").Replace(" ", "-")}",
                        Tokens = chunkTokens,
                        Content = chunk.Trim()
                    }
                });
            }
        }

        return documents;
    }

    private static int CountTokens(List<Message> messages, string model = "gpt-3.5-turbo-0613")
    {
        TikToken encoding = TikToken.GetEncoding("cl100k_base");

        int tokensPerMessage, tokensPerName;
        if (new List<string> { "gpt-3.5-turbo-0613", "gpt-3.5-turbo-16k-0613", "gpt-4-0314", "gpt-4-32k-0314", "gpt-4-0613", "gpt-4-32k-0613" }.Contains(model))
        {
            tokensPerMessage = 3;
            tokensPerName = 1;
        }
        else if (model == "gpt-3.5-turbo-0301")
        {
            tokensPerMessage = 4;
            tokensPerName = -1;
        }
        else if (model.Contains("gpt-3.5-turbo"))
        {
            Console.WriteLine("Warning: gpt-3.5-turbo may update over time. Returning num tokens assuming gpt-3.5-turbo-0613.");
            return CountTokens(messages);
        }
        else if (model.Contains("gpt-4"))
        {
            Console.WriteLine("Warning: gpt-4 may update over time. Returning num tokens assuming gpt-4-0613.");
            return CountTokens(messages, "gpt-4-0613");
        }
        else
        {
            throw new Exception($"num_tokens_from_messages() is not implemented for model {model}. See https://github.com/openai/openai-python/blob/main/chatml.md for information on how messages are converted to tokens.");
        }

        int numTokens = 0;
        foreach (Message message in messages)
        {
            numTokens += tokensPerMessage;
            
            // foreach (var entry in message)
            // {
            //     numTokens += encoding.Encode(entry.Value).Count;
            //     if (entry.Key == "name")
            //     {
            //         numTokens += tokensPerName;
            //     }
            // }

            foreach (PropertyInfo property in message.GetType().GetProperties())
            {
                string? value = property.GetValue(message)?.ToString();
                if (value is null)
                    continue;
                
                numTokens += encoding.Encode(value).Count;
                if (property.Name == "Name")
                {
                    numTokens += tokensPerName;
                }
            }
            
        }
        numTokens += 3;
        return numTokens;
    }
}
namespace AiDevsRag.Helpers;

public class Document
{
    public Document(string pageContent,
        Metadata metadata)
    {
        PageContent = pageContent;
        Metadata = metadata;
    }

    public string PageContent { get; }
    public Metadata Metadata { get; }
}
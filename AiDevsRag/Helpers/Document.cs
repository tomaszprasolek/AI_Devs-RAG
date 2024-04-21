namespace AiDevsRag.Helpers;

public class Document
{
    public string PageContent { get; set; }
    public Metadata Metadata { get; set; }

    public Document()
    {
        // Default constructor
    }

    public Document(string pageContent, Metadata metadata)
    {
        PageContent = pageContent;
        Metadata = metadata;
    }
}
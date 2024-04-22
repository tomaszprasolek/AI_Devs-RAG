namespace AiDevsRag.OpenAI.Embeddings;

public class Datum
{
    public string Object { get; set; }
    public int Index { get; set; }
    public List<double> Embedding { get; set; }
}
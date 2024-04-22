namespace AiDevsRag.OpenAI.Embeddings;

public class Embedding
{
    public string Object { get; set; }
    public List<Datum> Data { get; set; }
    public string Model { get; set; }
    public Usage Usage { get; set; }
}
namespace AiDevsRag.Qdrant.Scroll;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Result
{
    public List<object> points { get; set; }
    public object next_page_offset { get; set; }
}

public class ScrollResponse
{
    public Result result { get; set; }
    public string status { get; set; }
    public double time { get; set; }
}


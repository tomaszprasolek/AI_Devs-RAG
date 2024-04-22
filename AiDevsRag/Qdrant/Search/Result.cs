﻿using System.Text.Json.Serialization;

namespace AiDevsRag.Qdrant.Search;

public sealed class Result
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonPropertyName("payload")]
    public Payload Payload { get; set; }

    [JsonPropertyName("vector")]
    public object Vector { get; set; }
}
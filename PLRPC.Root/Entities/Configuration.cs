using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Entities;

public class Configuration
{
    [JsonPropertyName("serverUrl")]
    public string? ServerUrl { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }
}
using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Types;

public class PlrpcConfiguration
{
    [JsonPropertyName("serverUrl")]
    public string? ServerUrl { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }
}
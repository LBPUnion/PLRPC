using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Types.Configuration;

public class LocalConfiguration
{
    [JsonPropertyName("serverUrl")]
    public string ServerUrl { get; set; } = "https://lighthouse.lbpunion.com";

    [JsonPropertyName("username")]
    public string Username { get; set; } = "";
}
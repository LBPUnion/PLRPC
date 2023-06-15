using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Types;

public class PlrpcConfiguration
{
    [JsonPropertyName("serverUrl")]
    public string ServerUrl { get; set; } = "https://lighthouse.lbpunion.com";

    [JsonPropertyName("username")]
    public string Username { get; set; } = "";

    [JsonPropertyName("applicationId")]
    public string ApplicationId { get; set; } = "1060973475151495288";
}
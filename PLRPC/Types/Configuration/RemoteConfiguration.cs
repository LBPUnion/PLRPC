using System.Text.Json.Serialization;
using LBPUnion.PLRPC.Types.Enums;

namespace LBPUnion.PLRPC.Types.Configuration;

[Serializable]
public class RemoteConfiguration
{
    [JsonPropertyName("applicationId")]
    public string ApplicationId { get; set; } = "1060973475151495288";

    [JsonPropertyName("partyIdPrefix")]
    public string PartyIdPrefix { get; set; } = "project-lighthouse";

    [JsonPropertyName("usernameType")]
    public UsernameType UsernameType { get; set; } = UsernameType.Integer;

    [JsonPropertyName("assets")]
    public RpcAssets Assets { get; set; } = new();
}

[Serializable]
public class RpcAssets
{
    [JsonPropertyName("podAsset")]
    public string? PodAsset { get; set; }

    [JsonPropertyName("moonAsset")]
    public string? MoonAsset { get; set; }

    [JsonPropertyName("remoteMoonAsset")]
    public string? RemoteMoonAsset { get; set; }

    [JsonPropertyName("developerAsset")]
    public string? DeveloperAsset { get; set; }

    [JsonPropertyName("developerAdventureAsset")]
    public string? DeveloperAdventureAsset { get; set; }

    [JsonPropertyName("dlcAsset")]
    public string? DlcAsset { get; set; }

    [JsonPropertyName("fallbackAsset")]
    public string? FallbackAsset { get; set; }
}

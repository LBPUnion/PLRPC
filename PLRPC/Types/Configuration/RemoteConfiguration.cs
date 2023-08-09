using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Types.Configuration;

public class RemoteConfiguration
{
    [JsonPropertyName("applicationId")]
    public long ApplicationId { get; set; } = 1060973475151495288;

    [JsonPropertyName("partyIdPrefix")]
    public string? PartyIdPrefix { get; set; } = "project-lighthouse";

    [JsonPropertyName("usernameType")]
    public UsernameType UsernameType { get; set; } = UsernameType.Integer;

    [JsonPropertyName("assets")]
    public RpcAssets Assets { get; set; } = new();
}

public class RpcAssets
{
    public string? PodAsset { get; set; }
    public string? MoonAsset { get; set; }
    public string? RemoteMoonAsset { get; set; }
    public string? DeveloperAsset { get; set; }
    public string? DeveloperAdventureAsset { get; set; }
    public string? DlcAsset { get; set; }
    public string? FallbackAsset { get; set; }
}

// ReSharper disable UnusedMember.Global
public enum UsernameType
{
    Integer = 0,
    Username = 1,
}
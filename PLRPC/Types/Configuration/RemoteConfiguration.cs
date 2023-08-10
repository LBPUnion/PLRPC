using System.Text.Json.Serialization;
using JetBrains.Annotations;

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
    public string? PodAsset { get; [UsedImplicitly] set; }
    public string? MoonAsset { get; [UsedImplicitly] set; }
    public string? RemoteMoonAsset { get; [UsedImplicitly] set; }
    public string? DeveloperAsset { get; [UsedImplicitly] set; }
    public string? DeveloperAdventureAsset { get; [UsedImplicitly] set; }
    public string? DlcAsset { get; [UsedImplicitly] set; }
    public string? FallbackAsset { get; [UsedImplicitly] set; }
}

// ReSharper disable UnusedMember.Global
public enum UsernameType
{
    Integer = 0,
    Username = 1,
}
using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Types.Updater;

public class Manifest
{
    [JsonPropertyName("tag_name")]
    public string? TagName { get; init; }
}
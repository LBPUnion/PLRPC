using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Updater;

public class Manifest
{
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = null!;
}
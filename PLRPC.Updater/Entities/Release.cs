using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Updater;

public class Release
{
    [JsonPropertyName("html_url")]
    public string Url { get; set; } = null!;

    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = null!;
}
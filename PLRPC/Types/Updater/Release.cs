using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Types.Updater;

public class Release
{
    [JsonPropertyName("html_url")]
    public string? Url { get; set; }

    [JsonPropertyName("tag_name")]
    public string? TagName { get; set; }
}
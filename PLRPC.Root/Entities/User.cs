using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Entities;

public class User
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("yayHash")]
    public string? YayHash { get; set; }

    [JsonPropertyName("booHash")]
    public string? BooHash { get; set; }

    [JsonPropertyName("iconHash")]
    public string? IconHash { get; set; }

    [JsonPropertyName("lastLogin")]
    public Int64 LastLogin { get; set; }
}

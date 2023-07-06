using System.Text.Json.Serialization;
using LBPUnion.PLRPC.Types.Enums;
using Newtonsoft.Json;

namespace LBPUnion.PLRPC.Types.Entities;

[JsonObject]
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

    [JsonPropertyName("mehHash")]
    public string? MehHash { get; set; }

    [JsonPropertyName("lastLogin")]
    public long LastLogin { get; set; }

    [JsonPropertyName("permissionLevel")]
    public PermissionLevel? PermissionLevel { get; set; }
}
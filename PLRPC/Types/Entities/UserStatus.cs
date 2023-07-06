using System.Text.Json.Serialization;
using LBPUnion.PLRPC.Types.Enums;
using Newtonsoft.Json;

namespace LBPUnion.PLRPC.Types.Entities;

[JsonObject]
public class UserStatus
{
    [JsonPropertyName("statusType")]
    public StatusType StatusType { get; set; }

    [JsonPropertyName("currentVersion")]
    public GameVersion? CurrentVersion { get; set; }

    [JsonPropertyName("currentRoom")]
    public Room? CurrentRoom { get; set; }
}

public enum StatusType
{
    Offline = 0,
    Online = 1,
}
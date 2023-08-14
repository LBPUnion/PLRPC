using System.Text.Json.Serialization;
using LBPUnion.PLRPC.Types.Enums;

namespace LBPUnion.PLRPC.Types.Entities;

public class UserStatus
{
    [JsonPropertyName("statusType")]
    public StatusType StatusType { get; set; }

    [JsonPropertyName("currentVersion")]
    public GameVersion? CurrentVersion { get; set; }

    [JsonPropertyName("currentRoom")]
    public Room? CurrentRoom { get; set; }
}
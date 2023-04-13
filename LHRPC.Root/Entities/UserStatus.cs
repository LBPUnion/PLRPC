using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Entities;

public class UserStatus
{
    [JsonPropertyName("statusType")]
    public Types.StatusType StatusType { get; set; }

    [JsonPropertyName("currentVersion")]
    public Types.CurrentVersion? CurrentVersion { get; set; }

    [JsonPropertyName("currentRoom")]
    public Room? CurrentRoom { get; set; }
}

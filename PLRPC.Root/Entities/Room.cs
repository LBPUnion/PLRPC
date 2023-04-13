using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Entities;

public class Room
{
    [JsonPropertyName("roomId")]
    public int RoomId { get; set; }

    [JsonPropertyName("playerIds")]
    public string[]? PlayerIds { get; set; }

    [JsonPropertyName("slot")]
    public RoomSlot? RoomSlot { get; set; }
}

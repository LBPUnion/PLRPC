using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Entities;

public class Room
{
    [JsonPropertyName("roomId")]
    public int RoomId { get; set; }

    [JsonPropertyName("slot")]
    public RoomSlot? RoomSlot { get; set; }

    [JsonPropertyName("playerCount")]
    public int PlayerCount { get; set; }
}

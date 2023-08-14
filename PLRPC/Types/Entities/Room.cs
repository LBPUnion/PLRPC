using System.Text.Json.Serialization;
using LBPUnion.PLRPC.Types.Enums;

namespace LBPUnion.PLRPC.Types.Entities;

public class Room
{
    [JsonPropertyName("roomId")]
    public int RoomId { get; init; }

    [JsonPropertyName("playerIds")]
    public int[]? PlayerIds { get; init; }

    [JsonPropertyName("slot")]
    public RoomSlot? Slot { get; init; }
}

public class RoomSlot
{
    [JsonPropertyName("slotId")]
    public int SlotId { get; init; }

    [JsonPropertyName("slotType")]
    public SlotType SlotType { get; init; }
}
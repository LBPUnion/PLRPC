using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Types.Entities;

public class Room
{
    [JsonPropertyName("roomId")]
    public int RoomId { get; set; }

    [JsonPropertyName("playerIds")]
    public int[]? PlayerIds { get; set; }

    [JsonPropertyName("slot")]
    public RoomSlot? Slot { get; set; }
}

public class RoomSlot
{
    [JsonPropertyName("slotId")]
    public int SlotId { get; set; }

    [JsonPropertyName("slotType")]
    public SlotType SlotType { get; set; }
}
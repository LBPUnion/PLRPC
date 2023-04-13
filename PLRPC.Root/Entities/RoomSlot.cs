using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Entities;

public class RoomSlot
{
    [JsonPropertyName("slotId")]
    public int SlotId { get; set; }

    [JsonPropertyName("slotType")]
    public Types.SlotType SlotType { get; set; }
}

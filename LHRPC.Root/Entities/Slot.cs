using System.Text.Json.Serialization;

namespace LBPUnion.PLRPC.Entities;

public class Slot
{
    [JsonPropertyName("slotId")]
    public int SlotId { get; set; }

    [JsonPropertyName("name")]
    public string? SlotName { get; set; }

    [JsonPropertyName("iconHash")]
    public string? IconHash { get; set; }
}

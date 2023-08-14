using System.Text.Json.Serialization;
using LBPUnion.PLRPC.Types.Enums;

namespace LBPUnion.PLRPC.Types.Entities;

public class Slot
{
    [JsonPropertyName("slotId")]
    public int SlotId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("iconHash")]
    public string? IconHash { get; set; }

    [JsonPropertyName("type")]
    public SlotType SlotType { get; set; }
}
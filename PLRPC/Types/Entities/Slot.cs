using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LBPUnion.PLRPC.Types.Entities;

[JsonObject]
public class Slot
{
    [JsonPropertyName("slotId")]
    public int SlotId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("iconHash")]
    public string? IconHash { get; set; }

    [JsonPropertyName("type")]
    public SlotType Type { get; set; }
}

public enum SlotType
{
    Developer = 0,
    User = 1,
    Moon = 2,
    Unknown = 3,
    Unknown2 = 4,
    Pod = 5,
    DLC = 8,
}
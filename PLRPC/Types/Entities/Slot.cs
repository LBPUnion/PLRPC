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
    Unknown = -1,
    Developer = 0,
    User = 1,
    Moon = 2,
    // MoonGroup = 3,
    // DeveloperGroup = 4,
    Pod = 5,
    // Fake = 6,
    RemoteMoon = 7,
    DlcLevel = 8,
    // DLCPack = 9,
    // Playlist = 10,
    DeveloperAdventure = 11,
    // DeveloperAdventurePlanet = 12,
    // DeveloperAdventureArea = 13,
    // AdventurePlanetPublished = 14,
    // AdventurePlanetLocal = 15,
    // AdventureLevelLocal = 16,
    // AdventureAreaLevel = 17,
}
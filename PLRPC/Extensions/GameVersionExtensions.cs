using LBPUnion.PLRPC.Types;

namespace LBPUnion.PLRPC.Extensions;

public static class GameVersionExtensions
{
    public static string ToPrettyString(this GameVersion? version)
    {
        return version switch
        {
            GameVersion.LittleBigPlanet1 => "Playing LittleBigPlanet 1",
            GameVersion.LittleBigPlanet2 => "Playing LittleBigPlanet 2",
            GameVersion.LittleBigPlanet3 => "Playing LittleBigPlanet 3",
            GameVersion.LittleBigPlanetVita => "Playing LittleBigPlanet PS Vita",
            _ => ""
        };
    }
}

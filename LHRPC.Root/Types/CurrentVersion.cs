namespace LBPUnion.PLRPC.Types;

public enum CurrentVersion
{
    Offline = -1,
    LittleBigPlanet1 = 0,
    LittleBigPlanet2 = 1,
    LittleBigPlanet3 = 2,
    LittleBigPlanetVita = 3,
}

public static class GameVersionExtensions
{
    public static string String(this CurrentVersion? version)
    {
        return version switch
        {
            CurrentVersion.LittleBigPlanet1 => "Playing LittleBigPlanet 1",
            CurrentVersion.LittleBigPlanet2 => "Playing LittleBigPlanet 2",
            CurrentVersion.LittleBigPlanet3 => "Playing LittleBigPlanet 3",
            CurrentVersion.LittleBigPlanetVita => "Playing LittleBigPlanet PS Vita",
            CurrentVersion.Offline => "Account Offline",
            _ => ""
        };
    }
}

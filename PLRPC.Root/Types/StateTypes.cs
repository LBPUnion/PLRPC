namespace LBPUnion.PLRPC.Types;

public enum StatusType
{
    Offline = 0,
    Online = 1
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

public static class StateTypeExtensions
{
    public static string Status(this StatusType? statusType, Entities.UserStatus? userStatus)
    {
        return statusType switch
        {
            StatusType.Offline => "Playing LittleBigPlanet",
            StatusType.Online => $"{userStatus?.CurrentVersion.String()}",
            _ => "Unknown State"
        };
    }

    public static string Slot(this SlotType? slotType, Entities.Slot? slot)
    {
        return slotType switch
        {
            SlotType.User => $"{slot?.SlotName}",
            SlotType.Pod => "Dwelling in the Pod",
            SlotType.Moon => "Creating on the Moon",
            SlotType.Developer => "Playing a Story Level",
            SlotType.DLC => "Playing a DLC Level",
            _ => "Exploring the Imagisphere (っ◔◡◔)っ ❤"
        };
    }

    public static int Id(this SlotType? slotType, Entities.RoomSlot? slot)
    {
        return slotType switch
        {
            SlotType.User => slot?.SlotId ?? 0,
            SlotType.Pod => 1,
            SlotType.Moon => 2,
            SlotType.Developer => 3,
            SlotType.DLC => 4,
            _ => 5
        };
    }
}

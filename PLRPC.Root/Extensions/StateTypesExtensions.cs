using LBPUnion.PLRPC.Types;

namespace LBPUnion.PLRPC.Extensions;

public static class StateTypesExtensions
{
    public static string UserStatus(this StatusType? statusType, Entities.UserStatus? userStatus)
    {
        return statusType switch
        {
            StatusType.Online => $"{userStatus?.CurrentVersion.String()}",
            StatusType.Offline => "",
            _ => "Unknown State"
        };
    }

    public static string SlotName(this SlotType? slotType, Entities.Slot? slot)
    {
        return slotType switch
        {
            SlotType.User => $"{slot?.SlotName}",
            SlotType.Pod => "Dwelling in the Pod",
            SlotType.Moon => "Creating on the Moon",
            SlotType.Developer => "Playing a Story Level",
            SlotType.DLC => "Playing a DLC Level",
            _ => "(っ◔◡◔)っ ❤"
        };
    }

    public static int StaticId(this SlotType? slotType, Entities.RoomSlot? slot)
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

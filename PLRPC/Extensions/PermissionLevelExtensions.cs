using LBPUnion.PLRPC.Types.Enums;

namespace LBPUnion.PLRPC.Extensions;

public static class PermissionLevelExtensions
{
    public static string ToPrettyString(this PermissionLevel? level)
    {
        return level switch
        {
            PermissionLevel.Banned => " (Banned)",
            PermissionLevel.Restricted => " (Restricted)",
            PermissionLevel.Silenced => " (Silenced)",
            PermissionLevel.Default => "", // default PermissionLevel doesn't really have a pretty name
            PermissionLevel.Moderator => " (Moderator)",
            PermissionLevel.Administrator => " (Administrator)",
            _ => "",
        };
    }
}
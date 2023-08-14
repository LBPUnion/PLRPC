namespace LBPUnion.PLRPC.Types.Enums;

// ReSharper disable UnusedMember.Global
public enum UsernameType
{
    /// <summary>
    /// Instruct the client to use the integer ID as the username.
    /// Known use in Lighthouse-based servers.
    /// </summary>
    Integer = 0,

    /// <summary>
    /// Instruct the client to use the username string as the username.
    /// Known use in Refresh-based servers.
    /// </summary>
    Username = 1,
}
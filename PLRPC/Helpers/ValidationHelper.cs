using System.Text.RegularExpressions;
using Serilog;

namespace LBPUnion.PLRPC.Helpers;

public static partial class ValidationHelper
{
    public static bool IsValidUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out _)) return true;

        Log.Error("The URL specified is in an invalid format. Please try again");
        return false;
    }

    public static bool IsValidUsername(string username)
    {
        if (UsernameRegex().IsMatch(username)) return true;

        Log.Error("The username specified is invalid. Please try again");
        return false;
    }

    [GeneratedRegex("^[a-zA-Z0-9_.-]{3,16}$")]
    private static partial Regex UsernameRegex();
}
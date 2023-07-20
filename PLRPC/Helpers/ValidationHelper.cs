using System.Text.RegularExpressions;

namespace LBPUnion.PLRPC.Helpers;

public static partial class ValidationHelper
{
    public static bool IsValidUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out _);

    public static bool IsValidUsername(string username) => UsernameRegex().IsMatch(username);

    // Getting an error here? Roslyn issue - don't worry about it :)
    [GeneratedRegex("^[a-zA-Z0-9_.-]{3,16}$")]
    private static partial Regex UsernameRegex();
}
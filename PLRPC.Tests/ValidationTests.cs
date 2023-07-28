using LBPUnion.PLRPC.Helpers;
using Xunit;

namespace LBPUnion.PLRPC.Tests;

public class ValidationTests
{
    [Fact]
    public void PassValidUsername()
    {
        const string username = "littlebigmolly";

        bool isValidUsername = ValidationHelper.IsValidUsername(username);

        Assert.True(isValidUsername);
    }

    [Fact]
    public void PassValidServerUrl()
    {
        const string serverUrl = "https://lighthouse.lbpunion.com";

        bool isValidServerUrl = ValidationHelper.IsValidUrl(serverUrl);

        Assert.True(isValidServerUrl);
    }

    [Fact]
    public void FailInvalidUsername()
    {
        const string tooLongUsername = "InvalidUsernameThatIsTooLong";
        const string tooShortUsername = "a";
        const string symbolsUsername = "Inv@lidUsern@me";

        bool isValidLongUsername = ValidationHelper.IsValidUsername(tooLongUsername);
        bool isValidShortUsername = ValidationHelper.IsValidUsername(tooShortUsername);
        bool isValidSymbolsUsername = ValidationHelper.IsValidUsername(symbolsUsername);

        Assert.False(isValidLongUsername);
        Assert.False(isValidShortUsername);
        Assert.False(isValidSymbolsUsername);
    }

    // ReSharper disable once StringLiteralTypo
    // TODO: This could be improved later to add more invalid URL types
    [Fact]
    public void FailInvalidServerUrl()
    {
        const string invalidSchemeUrl = "https:lighthouse.lbpunion.com";

        bool isValidSchemeUrl = ValidationHelper.IsValidUrl(invalidSchemeUrl);

        Assert.False(isValidSchemeUrl);
    }
}
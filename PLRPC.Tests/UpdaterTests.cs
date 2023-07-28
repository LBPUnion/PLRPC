using System.Text.Json;
using LBPUnion.PLRPC.Types.Updater;
using Xunit;

namespace LBPUnion.PLRPC.Tests;

public class UpdaterTests
{
    [Fact]
    public async void CanGetLatestRelease()
    {
        HttpClient updaterHttpClient = new();

        // Required by GitHub's API
        updaterHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("LBPUnion/1.0 (PLRPC; unit-test) UpdateClient/1.1");

        string releaseManifest = await updaterHttpClient.GetStringAsync("https://api.github.com/repos/LBPUnion/PLRPC/releases/latest");
        Release? releaseObject = JsonSerializer.Deserialize<Release?>(releaseManifest);

        Assert.NotNull(releaseObject);

        Assert.Contains("v", releaseObject.TagName);
        Assert.Contains("https://github.com/LBPUnion/PLRPC/releases/tag/v", releaseObject.Url);
    }
}
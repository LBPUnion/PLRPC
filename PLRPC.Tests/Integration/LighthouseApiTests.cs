using LBPUnion.PLRPC.Types.Entities;
using LBPUnion.PLRPC.Types.Interfaces;
using Xunit;

namespace LBPUnion.PLRPC.Tests.Integration;

[Trait("Category", "Integration")]
public class LighthouseApiTests
{
    private static readonly HttpClient unitTestClient = new()
    {
        BaseAddress = new Uri("https://lighthouse.lbpunion.com/api/v1/"),
        DefaultRequestHeaders =
        {
            {
                "User-Agent", "LBPUnion/1.0 (PLRPC; unit-test) ApiClient/2.0"
            },
        },
    };

    private static readonly TimeSpan cacheExpirationTime = TimeSpan.FromHours(1);
    private static readonly LighthouseApiImpl lighthouseApi = new(unitTestClient, cacheExpirationTime);

    [Fact]
    public async void CanGetUser()
    {
        User? user = await lighthouseApi.GetUser("littlebigmolly");

        Assert.NotNull(user);

        Assert.Equal(992, user.UserId);
        Assert.Equal("littlebigmolly", user.Username);
    }

    // ReSharper disable once StringLiteralTypo
    [Fact]
    public async void CanGetSlot()
    {
        Slot? slot = await lighthouseApi.GetSlot(8443);

        Assert.NotNull(slot);

        Assert.Equal(8443, slot.SlotId);
        Assert.Equal(SlotType.User, slot.SlotType);

        Assert.Contains("Keystone Tower | LBP Union Ministrial Offices", slot.Name);
    }
}
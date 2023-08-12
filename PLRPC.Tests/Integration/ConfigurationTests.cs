using LBPUnion.PLRPC.Types.Configuration;
using LBPUnion.PLRPC.Types.Enums;
using Xunit;

namespace LBPUnion.PLRPC.Tests.Integration;

[Trait("Category", "Integration")]
public class ConfigurationTests
{
    private static readonly Logger unitTestLogger = new();
    
    private static readonly HttpClient lighthouseUnitTestClient = new()
    {
        BaseAddress = new Uri("https://lighthouse.lbpunion.com/api/v1/"),
        DefaultRequestHeaders = { { "User-Agent", "LBPUnion/1.0 (PLRPC; unit-test) ApiClient/2.0" } },
    };

    private static readonly HttpClient refreshUnitTestClient = new()
    {
        BaseAddress = new Uri("https://refresh.jvyden.xyz/api/v1/"),
        DefaultRequestHeaders = { { "User-Agent", "LBPUnion/1.0 (PLRPC; unit-test) ApiClient/2.0" } },
    };
    
    private static readonly Configuration lighthouseConfig = new(lighthouseUnitTestClient, unitTestLogger);
    private static readonly Configuration refreshConfig = new(refreshUnitTestClient, unitTestLogger);
    
    [Fact]
    public async void CanRetrieveAndParseLighthouseConfiguration()
    {
        RemoteConfiguration? remoteConfiguration = await lighthouseConfig.GetRemoteConfiguration();
        
        if (remoteConfiguration == null)
        {
            Assert.Fail("Failed to retrieve remote configuration");
        }
        
        Assert.Equal("1060973475151495288", remoteConfiguration.ApplicationId);
        Assert.Equal("beacon", remoteConfiguration.PartyIdPrefix);
        Assert.Equal(UsernameType.Integer, remoteConfiguration.UsernameType);
    }

    [Fact]
    public async void CanRetrieveAndParseRefreshConfiguration()
    {
        RemoteConfiguration? remoteConfiguration = await refreshConfig.GetRemoteConfiguration();

        if (remoteConfiguration == null)
        {
            Assert.Fail("Failed to retrieve remote configuration");
        }

        Assert.Equal("1138956002037866648", remoteConfiguration.ApplicationId);
        Assert.Equal("LittleBigRefresh", remoteConfiguration.PartyIdPrefix);
        Assert.Equal(UsernameType.Username, remoteConfiguration.UsernameType);
    }
}
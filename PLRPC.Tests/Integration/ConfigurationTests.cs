using LBPUnion.PLRPC.Types.Configuration;
using LBPUnion.PLRPC.Types.Enums;
using Xunit;

namespace LBPUnion.PLRPC.Tests.Integration;

[Trait("Category", "Integration")]
public class ConfigurationTests
{
    private static readonly HttpClient unitTestClient = new()
    {
        DefaultRequestHeaders =
        {
            {
                "User-Agent", "LBPUnion/1.0 (PLRPC; unit-test) ApiClient/2.0"
            },
        },
    };
    
    private static readonly Configuration lighthouseConfig = new(unitTestClient, new Logger());
    
    [Fact(Skip = "Remote configuration is not yet implemented into Lighthouse")]
    public async void CanRetrieveAndParseLighthouseConfiguration()
    {
        unitTestClient.BaseAddress = new Uri("https://lighthouse.lbpunion.com/api/v1/");
        
        RemoteConfiguration? remoteConfiguration = await lighthouseConfig.GetRemoteConfiguration();
        
        if (remoteConfiguration == null)
        {
            Assert.Fail("Failed to retrieve remote configuration");
        }

        // TODO: Add more conditions here when API is implemented to Beacon
        Assert.Equal("1060973475151495288", remoteConfiguration.ApplicationId);
    }

    [Fact]
    public async void CanRetrieveAndParseRefreshConfiguration()
    {
        unitTestClient.BaseAddress = new Uri("https://refresh.jvyden.xyz/api/v1/");
        
        RemoteConfiguration? remoteConfiguration = await lighthouseConfig.GetRemoteConfiguration();

        if (remoteConfiguration == null)
        {
            Assert.Fail("Failed to retrieve remote configuration");
        }

        Assert.Equal("1138956002037866648", remoteConfiguration.ApplicationId);
        Assert.Equal("LittleBigRefresh", remoteConfiguration.PartyIdPrefix);
        Assert.Equal(UsernameType.Username, remoteConfiguration.UsernameType);
    }
}
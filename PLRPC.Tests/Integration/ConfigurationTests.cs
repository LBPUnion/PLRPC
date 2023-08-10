using System.Text.Json;
using LBPUnion.PLRPC.Types.Configuration;
using Xunit;

namespace LBPUnion.PLRPC.Tests.Integration;

[Trait("Category", "Integration")]
public class ConfigurationTests
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

    private static readonly Configuration lighthouseConfig = new(unitTestClient, new Logger());
    
    [Fact]
    public async void CanRetrieveAndParseRemoteConfiguration()
    {
        RemoteConfiguration? remoteConfiguration = await lighthouseConfig.GetRemoteConfiguration();
        
        if (remoteConfiguration == null)
        {
            Assert.Fail("Failed to retrieve remote configuration");
        }

        // TODO: Add more conditions here when API is implemented to Beacon
        Assert.Equal(1060973475151495288, remoteConfiguration.ApplicationId);
    }
}
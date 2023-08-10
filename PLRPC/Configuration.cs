using System.Text.Json;
using LBPUnion.PLRPC.Types.Configuration;
using LBPUnion.PLRPC.Types.Logging;

namespace LBPUnion.PLRPC;

public class Configuration
{
    private readonly HttpClient httpClient;
    private readonly Logger logger;

    public Configuration(HttpClient httpClient, Logger logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<RemoteConfiguration?> GetRemoteConfiguration()
    {
        HttpResponseMessage remoteConfigReq = await this.httpClient.GetAsync("rpc");
        if (!remoteConfigReq.IsSuccessStatusCode) return null;

        RemoteConfiguration? remoteConfig =
            JsonSerializer.Deserialize<RemoteConfiguration>(await remoteConfigReq.Content.ReadAsStringAsync());

        if (remoteConfig != null) return remoteConfig;

        this.logger.Warning("Failed to deserialize remote configuration", LogArea.Configuration);
        return null;
    }
}
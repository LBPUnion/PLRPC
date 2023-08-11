using DiscordRPC;
using LBPUnion.PLRPC.Types.Configuration;
using LBPUnion.PLRPC.Types.Interfaces;
using LBPUnion.PLRPC.Types.Logging;
using LBPUnion.PLRPC.Types.Updater;

namespace LBPUnion.PLRPC;

// ReSharper disable once NotAccessedField.Local
public class Initializer
{
    private readonly Logger logger;
    private readonly Updater updater;

    public Initializer(Logger logger, Updater updater)
    {
        this.logger = logger;
        this.updater = updater;
    }

    public async Task InitializeLighthouseClient(string serverUrl, string username)
    {
        this.logger.Information("Initializing new client and dependencies", LogArea.LighthouseClient);

        #if !DEBUG
            await this.InitializeUpdateCheck();
        #endif

        string trimmedServerUrl = serverUrl.TrimEnd('/'); // trailing slashes cause issues with requests

        HttpClient apiClient = new()
        {
            BaseAddress = new Uri(trimmedServerUrl + "/api/v1/"),
            DefaultRequestHeaders = { { "User-Agent", "LBPUnion/1.0 (PLRPC; github-release) ApiClient/2.0" } },
        };

        TimeSpan cacheExpirationTime = TimeSpan.FromHours(1);

        LighthouseApiImpl lighthouseApi = new(apiClient, cacheExpirationTime);
        Configuration lighthouseConfig = new(apiClient, this.logger);

        RemoteConfiguration? remoteConfiguration = await lighthouseConfig.GetRemoteConfiguration();
        if (remoteConfiguration == null)
        {
            this.logger.Error("Failed to retrieve remote RPC configuration, is the instance up-to-date?", LogArea.Configuration);
            return;
        }

        DiscordRpcClient discordRpcClient = new(remoteConfiguration.ApplicationId.ToString());
        LighthouseClient lighthouseClient = new(username, trimmedServerUrl, lighthouseApi, remoteConfiguration, discordRpcClient, this.logger);

        await lighthouseClient.StartUpdateLoop();
    }

    // ReSharper disable once UnusedMember.Local
    private async Task InitializeUpdateCheck()
    {
        HttpClient updaterHttpClient = new();

        // Required by GitHub's API
        updaterHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("LBPUnion/1.0 (PLRPC; github-release) UpdateClient/1.1");

        this.logger.Information("Checking for updates", LogArea.Updater);

        Release? updateResult = await this.updater.CheckForUpdate(updaterHttpClient);

        if (updateResult != null)
        {
            this.logger.Information("A new version of PLRPC is available!", LogArea.Updater);
            this.logger.Information($"{updateResult.TagName}: {updateResult.Url}", LogArea.Updater);
        }
        else
        {
            this.logger.Information("There are no new updates available", LogArea.Updater);
        }
    }
}
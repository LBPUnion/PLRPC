using DiscordRPC;
using LBPUnion.PLRPC.Types.Logging;

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

    public async Task InitializeLighthouseClient(string serverUrl, string username, string? applicationId)
    {
        this.logger.Information("Initializing new client and dependencies", LogArea.LighthouseClient);

        #if !DEBUG
            await this.updater.InitializeUpdateCheck();
        #endif

        string trimmedServerUrl = serverUrl.TrimEnd('/'); // trailing slashes cause issues with requests

        HttpClient apiClient = new()
        {
            BaseAddress = new Uri(trimmedServerUrl + "/api/v1/"),
            DefaultRequestHeaders =
            {
                {
                    "User-Agent", "LBPUnion/1.0 (PLRPC; github-release) ApiClient/2.0"
                },
            },
        };

        TimeSpan cacheExpirationTime = TimeSpan.FromHours(1);

        ApiRepositoryImpl apiRepository = new(apiClient, cacheExpirationTime);
        DiscordRpcClient discordRpcClient = new(applicationId);
        LighthouseClient lighthouseClient = new(username, trimmedServerUrl, apiRepository, discordRpcClient, this.logger);

        await lighthouseClient.StartUpdateLoop();
    }
}
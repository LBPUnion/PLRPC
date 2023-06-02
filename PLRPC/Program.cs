using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CommandLine;
using DiscordRPC;
using LBPUnion.PLRPC.Helpers;
using LBPUnion.PLRPC.Logging;
using LBPUnion.PLRPC.Types;
using LBPUnion.PLRPC.Types.Updater;

namespace LBPUnion.PLRPC;

public static class Program
{
    public static async Task Main(string[] args)
    {
        #if !DEBUG
            await InitializeUpdateCheck();
        #endif

        await Parser.Default.ParseArguments<CommandLineArguments>(args).WithParsedAsync(ParseArguments);
    }

    private static async Task ParseArguments(CommandLineArguments arguments)
    {
        if (arguments.UseConfig)
        {
            PlrpcConfiguration? configuration = LoadFromConfiguration().Result;
            if (configuration is
                {
                    ServerUrl: not null,
                    Username: not null,
                })
                await InitializeLighthouseClient(configuration.ServerUrl, configuration.Username);
        }
        else if (arguments is
                 {
                     ServerUrl: not null,
                     Username: not null,
                 })
        {
            if (!ValidationHelper.IsValidUrl(arguments.ServerUrl)) return;
            if (!ValidationHelper.IsValidUsername(arguments.Username)) return;

            await InitializeLighthouseClient(arguments.ServerUrl, arguments.Username);
        }
        else
        {
            // We want to instruct the user to view the help if they don't pass any valid arguments.
            Logger.Error("No valid arguments passed. Please view --help for more information.");
        }
    }

    private static async Task<PlrpcConfiguration?> LoadFromConfiguration()
    {
        if (!File.Exists("./config.json"))
        {
            Logger.Warn("No configuration file exists, creating a base configuration.");
            Logger.Warn("Please populate the configuration file and restart the program.");
            PlrpcConfiguration defaultConfig = new()
            {
                ServerUrl = "https://lighthouse.lbpunion.com",
                Username = "",
            };
            await File.WriteAllTextAsync("./config.json",
                JsonSerializer.Serialize(defaultConfig,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    }));
            return null;
        }

        string configurationJson = await File.ReadAllTextAsync("./config.json");
        PlrpcConfiguration? configuration = JsonSerializer.Deserialize<PlrpcConfiguration>(configurationJson);

        if (configuration is
            {
                ServerUrl: not null,
                Username: not null,
            })
            return new PlrpcConfiguration
            {
                ServerUrl = configuration.ServerUrl,
                Username = configuration.Username,
            };
        Logger.Error("Configuration is invalid. Delete config.json and restart the program.");
        return null;
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static async Task InitializeUpdateCheck()
    {
        HttpClient updateClient = new();
        updateClient.DefaultRequestHeaders.UserAgent.ParseAdd("LBPUnion/1.0 (PLRPC; github-release) UpdateClient/1.1");
        Updater updater = new(updateClient);
        Release? updateResult = await updater.CheckForUpdate();
        if (updateResult != null)
        {
            Logger.Notice("***************************************");
            Logger.Notice("A new version of PLRPC is available!");
            Logger.Notice($"{updateResult.TagName}: {updateResult.Url}");
            Logger.Notice("***************************************");
        }
        else
        {
            Logger.Notice("There are no new updates available.");
        }
    }

    private static async Task InitializeLighthouseClient(string serverUrl, string username)
    {
        HttpClient apiClient = new()
        {
            BaseAddress = new Uri(serverUrl + "/api/v1/"),
            DefaultRequestHeaders =
            {
                {
                    "User-Agent", "LBPUnion/1.0 (PLRPC; github-release) ApiClient/2.0"
                },
            },
        };

        const int cacheExpirationTime = 60 * 60 * 1000; // 1 hour

        ApiRepositoryImpl apiRepository = new(apiClient, cacheExpirationTime);
        DiscordRpcClient discordRpcClient = new("1060973475151495288");
        LighthouseClient lighthouseClient = new(username, serverUrl, apiRepository, discordRpcClient);

        Logger.Info("Initializing client...");

        await lighthouseClient.StartUpdateLoop();
    }

    [Serializable]
    public class CommandLineArguments
    {
        [Option('c', "config", Required = false, HelpText = "Use a configuration file.")]
        public bool UseConfig { get; set; }

        [Option('s', "server", Required = false, HelpText = "The URL of the server to connect to.")]
        public string? ServerUrl { get; set; }

        [Option('u', "username", Required = false, HelpText = "Your username on the server.")]
        public string? Username { get; set; }
    }
}
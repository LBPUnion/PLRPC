using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using DiscordRPC;
using LBPUnion.PLRPC.Logging;
using LBPUnion.PLRPC.Types;
using LBPUnion.PLRPC.Types.Updater;

namespace LBPUnion.PLRPC;

public static partial class Program
{
    public static async Task Main(string[] args)
    {
        #if !DEBUG
            await ReleaseUpdateCheck();
        #endif

        string serverUrl;
        string username;

        if (args.Length > 0)
        {
            if (args[0] == "--config")
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
                    return;
                }

                string configurationJson = await File.ReadAllTextAsync("./config.json");
                PlrpcConfiguration? configuration = JsonSerializer.Deserialize<PlrpcConfiguration>(configurationJson);

                if (configuration?.ServerUrl == null || configuration.Username == null)
                {
                    Logger.Error("Configuration is invalid. Delete config.json and restart the program.");
                    return;
                }

                serverUrl = configuration.ServerUrl ?? "";
                username = configuration.Username ?? "";
            }
            else
            {
                Logger.Error("You have passed an invalid flag. You may use one of the following:");
                Logger.Error("  --config (to use a configuration file)");
                return;
            }
        }
        else
        {
            Console.Write("What is the URI of the Lighthouse Instance? (e.g. https://lighthouse.lbpunion.com) ");
            serverUrl = Console.ReadLine() ?? "";

            Console.Write("What is your registered username on this server? (e.g. littlebigmolly) ");
            username = Console.ReadLine() ?? "";
        }

        if (!UsernameRegex().IsMatch(username))
        {
            Logger.Error("The username specified is in an invalid format. Please try again.");
            return;
        }

        if (!Uri.TryCreate(serverUrl, UriKind.Absolute, out _))
        {
            Logger.Error("The URL specified is in an invalid format. Please try again.");
            return;
        }

        HttpClient apiClient = new()
        {
            BaseAddress = new Uri(serverUrl + "/api/v1/"),
            DefaultRequestHeaders =
            {
                {
                    "User-Agent", "PLRPC-Http-Updater/1.0"
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

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static async Task ReleaseUpdateCheck()
    {
        HttpClient updateClient = new();
        updateClient.DefaultRequestHeaders.UserAgent.ParseAdd("PLRPC-Http-Updater/1.0");
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

    [GeneratedRegex("^[a-zA-Z0-9_.-]{3,16}$")]
    private static partial Regex UsernameRegex();
}
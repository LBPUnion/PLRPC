using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CommandLine;
using DiscordRPC;
using JetBrains.Annotations;
using LBPUnion.PLRPC.Helpers;
using LBPUnion.PLRPC.Types;
using LBPUnion.PLRPC.Types.Logging;
using LBPUnion.PLRPC.Types.Updater;
using Serilog;
using Serilog.Core;

namespace LBPUnion.PLRPC;

public static class Program
{
    public static readonly Logger LogConfiguration = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.With<LogEnrichers>()
        .WriteTo.Console(outputTemplate: "[{ProcessId} {Timestamp:HH:mm:ss} {Level:u3}] {Message:l}{NewLine}{Exception}")
        .CreateLogger();

    private static readonly JsonSerializerOptions lenientJsonOptions = new()
    {
        AllowTrailingCommas = true,
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public static async Task Main(string[] args)
    {
        Log.Logger = LogConfiguration;

        #if !DEBUG
            await InitializeUpdateCheck();
        #endif

        await Parser.Default.ParseArguments<CommandLineArguments>(args).WithParsedAsync(ParseArguments);
    }

    private static async Task ParseArguments(CommandLineArguments arguments)
    {
        switch (arguments)
        {
            case { UseConfig: true }:
            {
                PlrpcConfiguration? configuration = LoadFromConfiguration().Result;
                if (configuration is { ServerUrl: not null, Username: not null, ApplicationId: not null })
                    await InitializeLighthouseClient(configuration.ServerUrl.TrimEnd('/'),
                        configuration.Username,
                        configuration.ApplicationId);
                break;
            }
            case { ServerUrl: not null, Username: not null } when !ValidationHelper.IsValidUrl(arguments.ServerUrl):
            case { ServerUrl: not null, Username: not null } when !ValidationHelper.IsValidUsername(arguments.Username):
                return;
            case { ServerUrl: not null, Username: not null, ApplicationId: not null }:
                await InitializeLighthouseClient(arguments.ServerUrl.TrimEnd('/'),
                    arguments.Username,
                    arguments.ApplicationId);
                break;
            default:
                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                Log.Error(arguments is { ServerUrl: null, Username: null, UseConfig: false }
                    ? "No arguments were passed to the client. Ensure you're running PLRPC through CLI"
                    : "Invalid argument(s) were passed to the client, please check them and try running again");
                Console.ReadLine();
                break;
        }
    }

    private static async Task<PlrpcConfiguration?> LoadFromConfiguration()
    {
        if (!File.Exists("./config.json"))
        {
            Log.Warning("No configuration file exists, creating a base configuration");
            Log.Warning("Please populate the configuration file and restart the program");

            PlrpcConfiguration defaultConfig = new();

            await File.WriteAllTextAsync("./config.json", JsonSerializer.Serialize(defaultConfig, lenientJsonOptions));

            return null;
        }

        string configurationJson = await File.ReadAllTextAsync("./config.json");

        try
        {
            PlrpcConfiguration? configuration =
                JsonSerializer.Deserialize<PlrpcConfiguration>(configurationJson, lenientJsonOptions);

            if (configuration is { ServerUrl: not null, Username: not null, ApplicationId: not null })
                return configuration;

            throw new JsonException("Deserialized configuration contains one or more null values");
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Failed to deserialize configuration file");
            return null;
        }
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
            Log.Information("{@Area}: A new version of PLRPC is available!", 
                LogArea.Updater);
            Log.Information("{@Area}: {UpdateTag}: {UpdateUrl}",
                LogArea.Updater, updateResult.TagName, updateResult.Url);
        }
        else
        {
            Log.Information("{@Area}: There are no new updates available", 
                LogArea.Updater);
        }
    }

    public static async Task InitializeLighthouseClient(string serverUrl, string username, string? applicationId)
    {
        Log.Information("{@Area}: Initializing new client and dependencies", 
            LogArea.LighthouseClient);

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
        DiscordRpcClient discordRpcClient = new(applicationId);
        LighthouseClient lighthouseClient = new(username, serverUrl, apiRepository, discordRpcClient);

        await lighthouseClient.StartUpdateLoop();
    }

    [UsedImplicitly]
    public class CommandLineArguments
    {
        public CommandLineArguments(bool useConfig, string? serverUrl, string? username, string? applicationId)
        {
            this.UseConfig = useConfig;
            this.ServerUrl = serverUrl;
            this.Username = username;
            this.ApplicationId = applicationId ?? "1060973475151495288"; // default to ProjectLighthouse app ID
        }

        [Option('c', "config", Required = false, HelpText = "Use a configuration file.")]
        public bool UseConfig { get; }

        [Option('s', "server", Required = false, HelpText = "The URL of the server to connect to.")]
        public string? ServerUrl { get; }

        [Option('u', "username", Required = false, HelpText = "Your username on the server.")]
        public string? Username { get; }

        [Option('a', "applicationid", Required = false, HelpText = "The Discord application ID to use.")]
        public string? ApplicationId { get; }
    }
}
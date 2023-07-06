using System.Diagnostics.CodeAnalysis;
using CommandLine;
using DiscordRPC;
using JetBrains.Annotations;
using LBPUnion.PLRPC.Helpers;
using LBPUnion.PLRPC.Types.Configuration;
using LBPUnion.PLRPC.Types.Logging;
using LBPUnion.PLRPC.Types.Updater;
using Serilog;
using Serilog.Core;

namespace LBPUnion.PLRPC;

public static class Program
{
    public static readonly Logger Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.With<LogEnrichers>()
        .WriteTo.Console(outputTemplate: "[{ProcessId} {Timestamp:HH:mm:ss} {Level:u3}] {Message:l}{NewLine}{Exception}")
        .CreateLogger();

    public static async Task Main(string[] args)
    {
        Log.Logger = Logger;

        #if !DEBUG
            await InitializeUpdateCheck();
        #endif

        await Parser.Default
            .ParseArguments<CommandLineArguments>(args)
            .WithParsedAsync(ProcessArguments);
    }

    // TODO: Make command line argument parsing more uniform and clean
    private static async Task ProcessArguments(CommandLineArguments arguments)
    {
        switch (arguments)
        {
            case { UseConfig: true }:
            {
                PlrpcConfiguration? configuration = Configuration.LoadFromConfiguration().Result;
                if (configuration is { ServerUrl: not null, Username: not null, ApplicationId: not null })
                    await InitializeLighthouseClient(configuration.ServerUrl, configuration.Username, configuration.ApplicationId);
                break;
            }
            case { ServerUrl: not null, Username: not null } when !ValidationHelper.IsValidUrl(arguments.ServerUrl):
            case { ServerUrl: not null, Username: not null } when !ValidationHelper.IsValidUsername(arguments.Username):
                return;
            case { ServerUrl: not null, Username: not null, ApplicationId: not null }:
                await InitializeLighthouseClient(arguments.ServerUrl, arguments.Username, arguments.ApplicationId);
                break;
            default:
                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                Log.Error(arguments is { ServerUrl: null, Username: null, UseConfig: false }
                    ? "{@Area}: No arguments were passed to the client. Ensure you're running PLRPC through CLI"
                    : "{@Area}: Invalid argument(s) were passed to the client, please check them and try running again",
                        LogArea.Configuration);
                Console.ReadLine();
                break;
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static async Task InitializeUpdateCheck()
    {
        HttpClient updateClient = new();
        Updater updater = new(updateClient);

        // Required by GitHub's API
        updateClient.DefaultRequestHeaders.UserAgent.ParseAdd("LBPUnion/1.0 (PLRPC; github-release) UpdateClient/1.1");

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

        const int cacheExpirationTime = 60 * 60 * 1000; // 1 hour

        ApiRepositoryImpl apiRepository = new(apiClient, cacheExpirationTime);
        DiscordRpcClient discordRpcClient = new(applicationId);
        LighthouseClient lighthouseClient = new(username, trimmedServerUrl, apiRepository, discordRpcClient);

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
using CommandLine;
using DiscordRPC;
using JetBrains.Annotations;
using LBPUnion.PLRPC.Helpers;
using LBPUnion.PLRPC.Types.Configuration;
using LBPUnion.PLRPC.Types.Logging;

namespace LBPUnion.PLRPC;

// ReSharper disable once UnusedMember.Local
public static class Program
{
    private static readonly Logger logger = new();
    private static readonly Updater updater = new(logger);
    private static readonly Configuration configuration = new(logger);

    public static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<CommandLineArguments>(args).WithParsedAsync(ProcessArguments);
    }

    // TODO: Make command line argument parsing more uniform and clean
    private static async Task ProcessArguments(CommandLineArguments arguments)
    {
        switch (arguments)
        {
            case { UseConfig: true }:
            {
                PlrpcConfiguration? plrpcConfiguration = configuration.LoadFromConfiguration().Result;
                if (plrpcConfiguration is { ServerUrl: not null, Username: not null, ApplicationId: not null })
                    await InitializeLighthouseClient(plrpcConfiguration.ServerUrl, plrpcConfiguration.Username,
                        plrpcConfiguration.ApplicationId);
                break;
            }
            case { ServerUrl: not null, Username: not null } when !ValidationHelper.IsValidUrl(arguments.ServerUrl):
            case { ServerUrl: not null, Username: not null } when !ValidationHelper.IsValidUsername(arguments.Username):
                logger.Error("The username or server URL you entered is not valid. Please try again.", LogArea.Validation);
                return;
            case { ServerUrl: not null, Username: not null, ApplicationId: not null }:
                await InitializeLighthouseClient(arguments.ServerUrl, arguments.Username, arguments.ApplicationId);
                break;
            default:
                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                logger.Error(arguments is { ServerUrl: null, Username: null, UseConfig: false }
                    ? "No arguments were passed to the client. Ensure you're running PLRPC through CLI"
                    : "Invalid argument(s) were passed to the client, please check them and try running again", LogArea.Configuration);
                Console.ReadLine();
                break;
        }
    }

    public static async Task InitializeLighthouseClient(string serverUrl, string username, string? applicationId)
    {
        logger.Information("Initializing new client and dependencies", LogArea.LighthouseClient);

        #if !DEBUG
            await updater.InitializeUpdateCheck();
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
        LighthouseClient lighthouseClient = new(username, trimmedServerUrl, apiRepository, discordRpcClient, logger);

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
using CommandLine;
using JetBrains.Annotations;
using LBPUnion.PLRPC.Helpers;
using LBPUnion.PLRPC.Types.Configuration;
using LBPUnion.PLRPC.Types.Logging;

namespace LBPUnion.PLRPC.CLI;

public static class Program
{
    private static readonly Logger logger = new();
    private static readonly Updater updater = new(logger);
    
    private static ParserResult<CommandLineArguments> parserResult;

    private static PlrpcConfiguration config;

    private static string serverUrl;
    private static string username;
    private static string applicationId;

    public static async Task Main(string[] args)
    {
        parserResult = Parser.Default.ParseArguments<CommandLineArguments>(args);

        await parserResult.MapResult(async arguments =>
        {
            switch (arguments)
            {
                case { UseConfig: true}: await RunConfigurationMode(); break;
                case { UseConfig: false }: await RunFlagMode(); break;
            }
        }, _ => Task.CompletedTask);
    }

    private static async Task RunConfigurationMode()
    {
        config = await new Configuration(logger).LoadFromConfiguration();

        if (config == null)
        {
            logger.Fatal("Failed to load configuration file, please check your entries.", LogArea.Configuration);
            return;
        }

        serverUrl = config.ServerUrl;
        username = config.Username;
        applicationId = config.ApplicationId;

        await new Initializer(logger, updater).InitializeLighthouseClient(serverUrl, username, applicationId);
    }

    private static async Task RunFlagMode()
    {
        parserResult.WithParsed(arguments =>
        {
            serverUrl = arguments.ServerUrl;
            username = arguments.Username;
            applicationId = arguments.ApplicationId;
        });

        bool isValidUsername = username != null && ValidationHelper.IsValidUsername(username);
        bool isValidServerUrl = serverUrl != null && ValidationHelper.IsValidUrl(serverUrl);

        if (!isValidUsername || !isValidServerUrl)
        {
            logger.Fatal("Server URL and/or username are invalid or were not specified.", LogArea.Configuration);
            return;
        }

        await new Initializer(logger, updater).InitializeLighthouseClient(serverUrl, username, applicationId);
    }

    [UsedImplicitly]
    #nullable enable
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
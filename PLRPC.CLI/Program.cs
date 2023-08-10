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

    private static string serverUrl;
    private static string username;

    public static async Task Main(string[] args)
    {
        parserResult = Parser.Default.ParseArguments<CommandLineArguments>(args);

        await parserResult.MapResult(async arguments =>
        {
            serverUrl = arguments.ServerUrl;
            username = arguments.Username;
            
            bool isValidUsername = username != null && ValidationHelper.IsValidUsername(username);
            bool isValidServerUrl = serverUrl != null && ValidationHelper.IsValidUrl(serverUrl);

            if (!isValidUsername || !isValidServerUrl)
            {
                logger.Fatal("Server URL and/or username are invalid or were not specified.", LogArea.Configuration);
                return;
            }

            await new Initializer(logger, updater).InitializeLighthouseClient(serverUrl, username);
        }, _ => Task.CompletedTask);
    }

    [UsedImplicitly]
    #nullable enable
    public class CommandLineArguments
    {
        public CommandLineArguments(string? serverUrl, string? username)
        {
            this.ServerUrl = serverUrl;
            this.Username = username;
        }

        [Option('s', "server", Required = true, HelpText = "The URL of the server to connect to.")]
        public string? ServerUrl { get; }

        [Option('u', "username", Required = true, HelpText = "Your username on the server.")]
        public string? Username { get; }
    }
}
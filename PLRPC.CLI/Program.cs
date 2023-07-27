using CommandLine;

namespace LBPUnion.PLRPC.CLI;

public static class Program
{
    private static ParserResult<Initializers.CommandLineArguments> result;

    public static async Task Main(string[] args)
    {
        result = Parser.Default.ParseArguments<Initializers.CommandLineArguments>(args);
        
        await result.MapResult(async arguments =>
        {
            await new Initializers().ProcessArguments(arguments);
        }, _ => Task.CompletedTask);
    }
}
using CommandLine;

namespace LBPUnion.PLRPC.CLI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        ParserResult<Initializers.CommandLineArguments> result = Parser.Default.ParseArguments<Initializers.CommandLineArguments>(args);
        await result.MapResult(async arguments => await new Initializers().ProcessArguments(arguments), _ => Task.CompletedTask);
    }
}
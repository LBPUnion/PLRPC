using LBPUnion.PLRPC.Types.Logging;

namespace LBPUnion.PLRPC.Logging;

public static class Logger
{
    private static void Log(LogLevel level, string message)
    {
        Console.WriteLine($"<PLRPC> {DateTime.Now} {level.ToString().ToUpper()} {message}");
    }

    public static void Notice(string message) => Log(LogLevel.Notice, message);
    public static void Info(string message) => Log(LogLevel.Info, message);
    public static void Warn(string message) => Log(LogLevel.Warn, message);
    public static void Error(string message) => Log(LogLevel.Error, message);

    public static void LogException(Exception exception)
    {
        string errorMsg = @$"
        *** PLRPC has experienced an error and will now exit. ***
        This is most likely *not your fault*. Try restarting the client and check your configuration.
        If this error persists, please create a new GitHub issue using the Bug Report template.
        
        {exception.Message}
        
        ";
        Error(errorMsg);
    }
}
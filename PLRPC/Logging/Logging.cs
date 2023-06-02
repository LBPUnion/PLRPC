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
        Error("A severe error has occurred. PLRPC will now exit.");
        Error($"↳ {exception.GetType().Name}: {exception.Message}");
    }
}
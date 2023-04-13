namespace LBPUnion.PLRPC.Logging;

public static class Message
{
    public static void Info(string message)
    {
        Console.WriteLine($"<PLRPC> {DateTime.Now.ToString()} [INFO] {message}");
    }

    public static void Warn(string message)
    {
        Console.WriteLine($"<PLRPC> {DateTime.Now.ToString()} [WARN] {message}");
    }

    public static void Error(string message)
    {
        Console.WriteLine($"<PLRPC> {DateTime.Now.ToString()} [ERROR] {message}");
    }
}

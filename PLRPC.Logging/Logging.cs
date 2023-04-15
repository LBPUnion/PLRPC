namespace LBPUnion.PLRPC.Logging;

public static class Message
{
    public static void New(int type, string message)
    {
        string Log = type switch
        {
            0 => $"<PLRPC> {DateTime.Now.ToString()} [INFO] {message}",
            1 => $"<PLRPC> {DateTime.Now.ToString()} [NOTICE] {message}",
            2 => $"<PLRPC> {DateTime.Now.ToString()} [WARN] {message}",
            3 => $"<PLRPC> {DateTime.Now.ToString()} [ERROR] {message}",
            _ => $"<PLRPC> {DateTime.Now.ToString()} {message}"
        };
        Console.WriteLine(Log);
    }

    public static void Exception(string exception)
    {
        Logging.Message.New(3, $"");
        Logging.Message.New(3, $"*** PLRPC has experienced an error and will now exit. ***");
        Logging.Message.New(3, $"This is most likely *not your fault*. Try restarting the client and check your configuration.");
        Logging.Message.New(3, $"If this error persists, please create a new GitHub issue using the Bug Report template.");
        Logging.Message.New(3, $"");
        Logging.Message.New(3, $"{exception}");
        Logging.Message.New(3, $"");
    }
}

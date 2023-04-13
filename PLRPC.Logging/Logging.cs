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
}

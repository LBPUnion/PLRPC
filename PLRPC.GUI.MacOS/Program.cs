namespace LBPUnion.PLRPC.GUI;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        /*
         * Attempt to set the current directory to the directory of the app bundle as
         * macOS application bundles have their default pwd set to /.
         *
         * We'll just fall back to "../" if this fails.
         */
        Directory.SetCurrentDirectory(Path.GetDirectoryName(Environment.ProcessPath) ?? "../");
        
        Gui.Initialize();
    }
}
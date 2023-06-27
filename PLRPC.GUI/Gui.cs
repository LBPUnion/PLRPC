using Eto.Forms;
using LBPUnion.PLRPC.GUI.Forms;

namespace LBPUnion.PLRPC.GUI;

public static class Gui
{
    public static void Initialize()
    {
        new Application().Run(new MainForm());
    }
}
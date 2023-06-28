namespace LBPUnion.PLRPC.GUI.Strings;

public static class MainForm
{
    public static string String(this MainFormStrings strings)
    {
        return strings switch
        {
            MainFormStrings.Configuration => "Configuration",
            MainFormStrings.Username => "Username",
            MainFormStrings.ServerUrl => "Server URL",
            MainFormStrings.ApplicationId => "Application ID",
            MainFormStrings.Connect => "Connect",
            MainFormStrings.Connected => "Connected",
            MainFormStrings.UnlockDefaults => "Unlock Defaults",
            MainFormStrings.UnlockedDefaults => "Unlocked Defaults",
            MainFormStrings.BlankFieldsError => "Please fill in all fields and try again.",
            MainFormStrings.InitializationError => "An error occurred while initializing the PLRPC client.",
            MainFormStrings.UnlockedDefaultsWarning => "You have just unlocked defaults. Support will not be provided whilst using modified defaults. Continue at your own risk.",
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}

public enum MainFormStrings
{
    Configuration,
    Username,
    ServerUrl,
    ApplicationId,
    Connect,
    Connected,
    UnlockDefaults,
    UnlockedDefaults,
    BlankFieldsError,
    InitializationError,
    UnlockedDefaultsWarning,
}
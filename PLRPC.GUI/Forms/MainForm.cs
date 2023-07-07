using System.Text;
using Eto.Drawing;
using Eto.Forms;
using LBPUnion.PLRPC.GUI.Strings;
using LBPUnion.PLRPC.Types.Logging;
using Serilog;

namespace LBPUnion.PLRPC.GUI.Forms;

public class MainForm : Form
{
    private static readonly TextBox username;
    private static readonly TextBox serverUrl;
    private static readonly TextBox applicationId;

    public MainForm()
    {
        this.Title = "PLRPC";
        this.ClientSize = new Size(400, -1);
        this.Resizable = false;

        this.Content = this.tableLayout;

        Log.Logger = Program.Logger;
    }

    private static readonly GroupBox configurationEntries = new()
    {
        Text = Strings.MainForm.Configuration,
        Content = new TableLayout
        {
            Padding = new Padding(3, 3, 3, 3),
            Spacing = new Size(3, 3),
            Rows =
            {
                new TableRow(new List<TableCell>
                {
                    new(new Label
                    {
                        Text = Strings.MainForm.Username,
                    }),
                    new(username = new TextBox()),
                }),
                new TableRow(new List<TableCell>
                {
                    new(new Label
                    {
                        Text = Strings.MainForm.ServerUrl,
                    }),
                    new(serverUrl = new TextBox
                    {
                        Text = "https://lighthouse.lbpunion.com/",
                        Enabled = false,
                    }),
                }),
                new TableRow(new List<TableCell>
                {
                    new(new Label
                    {
                        Text = Strings.MainForm.ApplicationId,
                    }),
                    new(applicationId = new TextBox
                    {
                        Text = "1060973475151495288",
                        Enabled = false,
                    }),
                }),
            },
        },
    };

    private static readonly Button connectButton = new(InitializeClientHandler)
    {
        Text = Strings.MainForm.Connect,
    };

    private static readonly Button unlockDefaultsButton = new(UnlockDefaultsHandler)
    {
        Text = Strings.MainForm.UnlockDefaults,
    };

    private readonly TableLayout tableLayout = new()
    {
        Padding = new Padding(10, 10, 10, 10),
        Spacing = new Size(5, 5),
        Rows =
        {
            new TableRow(configurationEntries),
            new TableRow(connectButton),
            new TableRow(unlockDefaultsButton),
        },
    };

    private static async void InitializeClientHandler(object sender, EventArgs eventArgs)
    {
        List<TextBox> arguments = new()
        {
            serverUrl,
            username,
            applicationId,
        };

        if (arguments.Any(a => string.IsNullOrWhiteSpace(a.Text)))
        {
            MessageBox.Show(Strings.MainForm.BlankFieldsError, MessageBoxButtons.OK, MessageBoxType.Error);
            return;
        }

        try
        {
            // Text changes
            connectButton.Text = Strings.MainForm.Connected;

            // Button states
            connectButton.Enabled = false;
            unlockDefaultsButton.Enabled = false;

            // Field states
            serverUrl.Enabled = false;
            username.Enabled = false;
            applicationId.Enabled = false;

            await Program.InitializeLighthouseClient(serverUrl.Text.Trim('/'), username.Text, applicationId.Text);
        }
        catch (Exception exception)
        {
            StringBuilder exceptionBuilder = new();

            exceptionBuilder.AppendLine($"{Strings.MainForm.InitializationError}\n");
            exceptionBuilder.AppendLine($"{exception.Message}\n");
            exceptionBuilder.AppendLine($"{exception.Source}");

            Log.Error(exception, "{@Area}: Failed to initialize the client",
                LogArea.LighthouseClient);

            MessageBox.Show(exceptionBuilder.ToString(), MessageBoxButtons.OK, MessageBoxType.Error);
        }
    }

    private static void UnlockDefaultsHandler(object sender, EventArgs eventArgs)
    {
        // Text changes
        unlockDefaultsButton.Text = Strings.MainForm.UnlockedDefaults;

        // Button states
        unlockDefaultsButton.Enabled = false;

        // Field states
        serverUrl.Enabled = true;
        applicationId.Enabled = true;

        MessageBox.Show(Strings.MainForm.UnlockedDefaultsWarning,
            "Warning",
            MessageBoxButtons.OK,
            MessageBoxType.Warning);
    }
}
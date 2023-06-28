using System.Text;
using Eto.Drawing;
using Eto.Forms;
using LBPUnion.PLRPC.GUI.Strings;
using LBPUnion.PLRPC.Types;
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

        Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
            .Enrich.With<LogEnrichers>()
            .WriteTo.Console(
                outputTemplate: "[{ProcessId} {Timestamp:HH:mm:ss} {Level:u3}] {Message:l}{NewLine}{Exception}")
            .CreateLogger();
    }

    #region GUI

    private static readonly GroupBox configurationEntries = new()
    {
        Text = MainFormStrings.Configuration.String(),
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
                        Text = MainFormStrings.Username.String(),
                    }),
                    new(username = new TextBox()),
                }),
                new TableRow(new List<TableCell>
                {
                    new(new Label
                    {
                        Text = MainFormStrings.ServerUrl.String(),
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
                        Text = MainFormStrings.ApplicationId.String(),
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
        Text = MainFormStrings.Connect.String(),
    };

    private static readonly Button unlockDefaultsButton = new(UnlockDefaultsHandler)
    {
        Text = MainFormStrings.UnlockDefaults.String(),
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

    #endregion

    #region Event handlers

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
            MessageBox.Show(MainFormStrings.BlankFieldsError.String(), MessageBoxButtons.OK, MessageBoxType.Error);
            return;
        }

        try
        {
            connectButton.Text = MainFormStrings.Connected.String();

            connectButton.Enabled = false;
            unlockDefaultsButton.Enabled = false;

            serverUrl.Enabled = false;
            username.Enabled = false;
            applicationId.Enabled = false;

            await Program.InitializeLighthouseClient(serverUrl.Text.Trim('/'), username.Text, applicationId.Text);
        }
        catch (Exception exception)
        {
            StringBuilder exceptionBuilder = new();

            exceptionBuilder.AppendLine($"{MainFormStrings.InitializationError.String()}\n");
            exceptionBuilder.AppendLine($"{exception.Message}\n");
            exceptionBuilder.AppendLine($"{exception.Source}");

            Log.Error(exception, "Failed to initialize the client");

            MessageBox.Show(exceptionBuilder.ToString(), MessageBoxButtons.OK, MessageBoxType.Error);
        }
    }

    private static void UnlockDefaultsHandler(object sender, EventArgs eventArgs)
    {
        unlockDefaultsButton.Enabled = false;

        serverUrl.Enabled = true;
        applicationId.Enabled = true;

        unlockDefaultsButton.Text = MainFormStrings.UnlockedDefaults.String();
        unlockDefaultsButton.BackgroundColor = Colors.Pink;

        MessageBox.Show(MainFormStrings.UnlockedDefaultsWarning.String(),
            "Warning",
            MessageBoxButtons.OK,
            MessageBoxType.Warning);
    }

    #endregion
}
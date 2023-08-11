using System.Text;
using Eto.Drawing;
using Eto.Forms;
using LBPUnion.PLRPC.Helpers;

namespace LBPUnion.PLRPC.GUI.Forms;

public sealed class MainForm : Form
{
    private static readonly Logger logger = new();
    private static readonly Updater updater = new(logger);
    
    private static readonly TextBox username;
    private static readonly TextBox serverUrl;

    public MainForm()
    {
        this.Title = "PLRPC";
        this.ClientSize = new Size(400, -1);
        this.Resizable = false;

        this.Content = this.tableLayout;
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
                    }),
                }),
            },
        },
    };

    private static readonly Button connectButton = new(InitializationHandler)
    {
        Text = Strings.MainForm.Connect,
    };

    private readonly TableLayout tableLayout = new()
    {
        Padding = new Padding(10, 10, 10, 10),
        Spacing = new Size(5, 5),
        Rows =
        {
            new TableRow(configurationEntries),
            new TableRow(connectButton),
        },
    };

    private static async void InitializationHandler(object sender, EventArgs eventArgs)
    {
        List<TextBox> arguments = new()
        {
            serverUrl,
            username,
        };

        switch (arguments)
        {
            case not null when arguments.Any(a => string.IsNullOrWhiteSpace(a.Text)):
            {
                MessageBox.Show(Strings.MainForm.BlankFieldsError, MessageBoxButtons.OK, MessageBoxType.Error);
                return;
            }
            case not null when !ValidationHelper.IsValidUsername(username.Text):
            {
                MessageBox.Show(Strings.MainForm.InvalidUsernameError, MessageBoxButtons.OK, MessageBoxType.Error);
                return;
            }
            case not null when !ValidationHelper.IsValidUrl(serverUrl.Text):
            {
                MessageBox.Show(Strings.MainForm.InvalidUrlError, MessageBoxButtons.OK, MessageBoxType.Error);
                return;
            }
        }

        // Text changes
        connectButton.Text = Strings.MainForm.Connected;

        // Button states
        connectButton.Enabled = false;

        // Field states
        serverUrl.Enabled = false;
        username.Enabled = false;

        try
        {
            await new Initializer(logger, updater).InitializeLighthouseClient(serverUrl.Text.Trim('/'), username.Text);
        }
        catch (Exception exception)
        {
            StringBuilder exceptionBuilder = new();

            exceptionBuilder.AppendLine($"{Strings.MainForm.InitializationError}\n");
            exceptionBuilder.AppendLine($"{exception.Message}\n");
            exceptionBuilder.AppendLine($"{exception.Source}");

            MessageBox.Show(exceptionBuilder.ToString(), MessageBoxButtons.OK, MessageBoxType.Error);
        }
    }
}
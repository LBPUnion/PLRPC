using System.Text.Json;
using LBPUnion.PLRPC.Types.Configuration;
using LBPUnion.PLRPC.Types.Logging;

namespace LBPUnion.PLRPC;

public class Configuration
{
    private static readonly JsonSerializerOptions lenientJsonOptions = new()
    {
        AllowTrailingCommas = true,
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    private readonly Logger logger;

    public Configuration(Logger logger)
    {
        this.logger = logger;
    }

    public async Task<LocalConfiguration?> LoadFromConfiguration()
    {
        if (!File.Exists("./config.json"))
        {
            this.logger.Warning("No configuration file exists, creating a base configuration", LogArea.Configuration);
            this.logger.Warning("Please populate the configuration file and restart the program", LogArea.Configuration);

            LocalConfiguration defaultConfig = new();

            await File.WriteAllTextAsync("./config.json", JsonSerializer.Serialize(defaultConfig, lenientJsonOptions));

            return null;
        }

        string configurationJson = await File.ReadAllTextAsync("./config.json");
        LocalConfiguration? configuration = JsonSerializer.Deserialize<LocalConfiguration>(configurationJson, lenientJsonOptions);

        return configuration is { ServerUrl: not null, Username: not null }
            ? configuration
            : null;
    }
}
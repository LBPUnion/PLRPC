using System.Text.Json;
using LBPUnion.PLRPC.Types.Configuration;
using LBPUnion.PLRPC.Types.Logging;
using Serilog;

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

    public async Task<PlrpcConfiguration?> LoadFromConfiguration()
    {
        if (!File.Exists("./config.json"))
        {
            this.logger.Warning("No configuration file exists, creating a base configuration", LogArea.Configuration);
            this.logger.Warning("Please populate the configuration file and restart the program", LogArea.Configuration);

            PlrpcConfiguration defaultConfig = new();

            await File.WriteAllTextAsync("./config.json", JsonSerializer.Serialize(defaultConfig, lenientJsonOptions));

            return null;
        }

        string configurationJson = await File.ReadAllTextAsync("./config.json");

        try
        {
            PlrpcConfiguration? configuration = JsonSerializer.Deserialize<PlrpcConfiguration>(configurationJson, lenientJsonOptions);

            if (configuration is { ServerUrl: not null, Username: not null, ApplicationId: not null })
                return configuration;

            throw new JsonException("Deserialized configuration contains one or more null values");
        }
        catch (Exception exception)
        {
            this.logger.Fatal("Failed to deserialize configuration file", LogArea.Configuration, exception);
            return null;
        }
    }
}
using System.Text.Json;
using LBPUnion.PLRPC.Types.Configuration;
using Xunit;

namespace LBPUnion.PLRPC.Tests;

public class ConfigurationTests
{
    private static readonly JsonSerializerOptions lenientJsonOptions = new()
    {
        AllowTrailingCommas = true,
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };
    
    [Fact]
    public async void CanGenerateConfiguration()
    {
        PlrpcConfiguration defaultConfig = new();
        await File.WriteAllTextAsync("./config.json", JsonSerializer.Serialize(defaultConfig, lenientJsonOptions));

        bool configExists = File.Exists("./config.json");

        Assert.True(configExists);
    }

    [Fact]
    public async void CanParseConfiguration()
    {
        string configurationJson = await File.ReadAllTextAsync("./config.json");
        PlrpcConfiguration? configuration = JsonSerializer.Deserialize<PlrpcConfiguration>(configurationJson, lenientJsonOptions);

        bool configurationIsValid = configuration is { ServerUrl: not null, Username: not null, ApplicationId: not null };

        Assert.True(configurationIsValid);
    }
}
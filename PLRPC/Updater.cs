using System.Text.Json;
using LBPUnion.PLRPC.Types.Logging;
using LBPUnion.PLRPC.Types.Updater;

namespace LBPUnion.PLRPC;

public class Updater
{
    private readonly Logger logger;

    public Updater(Logger logger)
    {
        this.logger = logger;
    }

    // ReSharper disable once UnusedMember.Global
    public async Task InitializeUpdateCheck()
    {
        HttpClient updaterHttpClient = new();

        // Required by GitHub's API
        updaterHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("LBPUnion/1.0 (PLRPC; github-release) UpdateClient/1.1");

        this.logger.Information("Checking for updates", LogArea.Updater);

        Release? updateResult = await this.CheckForUpdate(updaterHttpClient);

        if (updateResult != null)
        {
            this.logger.Information("A new version of PLRPC is available!", LogArea.Updater);
            this.logger.Information($"{updateResult.TagName}: {updateResult.Url}", LogArea.Updater);
        }
        else
        {
            this.logger.Information("There are no new updates available", LogArea.Updater);
        }
    }

    private async Task<Release?> CheckForUpdate(HttpClient updaterHttpClient)
    {
        if (!File.Exists("./manifest.json"))
        {
            this.logger.Warning("No update manifest file exists, creating a base manifest", LogArea.Updater);
            await GenerateManifest(updaterHttpClient);
        }

        string releaseManifest = await updaterHttpClient.GetStringAsync("https://api.github.com/repos/LBPUnion/PLRPC/releases/latest");
        string programManifest = await File.ReadAllTextAsync("./manifest.json");

        Release? releaseObject = JsonSerializer.Deserialize<Release?>(releaseManifest);
        Manifest? programObject = JsonSerializer.Deserialize<Manifest?>(programManifest);

        if (releaseObject == null || programObject == null || releaseObject.TagName == programObject.TagName)
            return null;

        return releaseObject;
    }

    private static async Task GenerateManifest(HttpClient updaterHttpClient)
    {
        string currentManifest = await updaterHttpClient.GetStringAsync("https://api.github.com/repos/LBPUnion/PLRPC/releases/latest");

        Release? currentRelease = JsonSerializer.Deserialize<Release>(currentManifest);

        if (currentRelease == null) return;

        Manifest baseManifest = new()
        {
            TagName = currentRelease.TagName,
        };

        await File.WriteAllTextAsync("./manifest.json", JsonSerializer.Serialize(baseManifest, new JsonSerializerOptions
        {
            WriteIndented = true,
        }));
    }
}
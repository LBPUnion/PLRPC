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

    public async Task<Release?> CheckForUpdate(HttpClient updaterHttpClient)
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

        updaterHttpClient.Dispose();
        
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
using System.Text.Json;
using LBPUnion.PLRPC.Logging;
using LBPUnion.PLRPC.Types.Updater;

namespace LBPUnion.PLRPC;

public class Updater
{
    private readonly HttpClient updaterHttpClient;

    public Updater(HttpClient updaterClient)
    {
        this.updaterHttpClient = updaterClient;
    }

    public async Task<Release?> CheckForUpdate()
    {
        if (!File.Exists("./manifest.json"))
        {
            Logger.Warn("No update manifest file exists, creating a base manifest.");
            await this.GenerateManifest();
        }

        string releaseManifest =
            await this.updaterHttpClient.GetStringAsync("https://api.github.com/repos/LBPUnion/PLRPC/releases/latest");
        string programManifest = await File.ReadAllTextAsync("./manifest.json");

        Release? releaseObject = JsonSerializer.Deserialize<Release?>(releaseManifest);
        Manifest? programObject = JsonSerializer.Deserialize<Manifest?>(programManifest);

        if (releaseObject == null || programObject == null || releaseObject.TagName == programObject.TagName)
            return null;

        return releaseObject;
    }

    private async Task GenerateManifest()
    {
        string currentManifest =
            await this.updaterHttpClient.GetStringAsync("https://api.github.com/repos/LBPUnion/PLRPC/releases/latest");

        Release? currentRelease = JsonSerializer.Deserialize<Release>(currentManifest);

        if (currentRelease == null) return;

        Manifest baseManifest = new()
        {
            TagName = currentRelease.TagName,
        };

        await File.WriteAllTextAsync("./manifest.json",
            JsonSerializer.Serialize(baseManifest,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                }));
    }
}
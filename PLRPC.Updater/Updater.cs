using System.Text.Json;

namespace LBPUnion.PLRPC.Updater;

public static class Updater
{
    public static HttpClient UpdaterHttpClient = null!;

    public static async Task<Release?> CheckUpdate()
    {
        UpdaterHttpClient = new HttpClient();
        UpdaterHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("PLRPC-Http-Updater/1.0");

        if (!File.Exists("./manifest.json"))
        {
            Logging.Message.New(2, "No update manifest file exists, creating a base manifest.");
            await GenerateManifest();
        }

        string releaseManifest = await UpdaterHttpClient.GetStringAsync("https://api.github.com/repos/LBPUnion/PLRPC/releases/latest");
        string programManifest = File.ReadAllText("./manifest.json");

        Release? releaseObject = JsonSerializer.Deserialize<Release?>(releaseManifest);
        Manifest? programObject = JsonSerializer.Deserialize<Manifest?>(programManifest);

        if (releaseObject == null)
        {
            return null;
        }

        if (programObject == null)
        {
            return null;
        }

        if (releaseObject.TagName == programObject.TagName)
        {
            return null;
        }

        return releaseObject;
    }

    public static async Task GenerateManifest()
    {
        UpdaterHttpClient = new HttpClient();
        UpdaterHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("PLRPC-Http-Updater/1.0");

        string currentManifest = await UpdaterHttpClient.GetStringAsync("https://api.github.com/repos/LBPUnion/PLRPC/releases/latest");

        Release? currentRelease = JsonSerializer.Deserialize<Release>(currentManifest);

        if (currentRelease == null)
        {
            return;
        }

        Manifest? BaseManifest = new()
        {
            TagName = currentRelease.TagName
        };

        await File.WriteAllTextAsync("./manifest.json", JsonSerializer.Serialize(BaseManifest, new JsonSerializerOptions { WriteIndented = true }));
    }
}
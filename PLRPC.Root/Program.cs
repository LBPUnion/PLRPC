using DiscordRPC;
using DiscordRPC.Logging;
using System.Text.Json;

namespace LBPUnion.PLRPC.Main;

public static class Program
{
    public static readonly DiscordRpcClient DiscordClient = new("1060973475151495288");
    public static readonly Dictionary<string, Entities.User?> UserCache = new();
    public static readonly Dictionary<int, Entities.Slot?> SlotCache = new();
    public static HttpClient APIHttpClient = null!;
    public static HttpClient AssetsHttpClient = null!;
    public static string username = null!;
    public static string serverUrl = null!;

    public static async Task Main()
    {
        Console.Write("What is the URI of the Lighthouse Instance? (e.g. https://lighthouse.lbpunion.com) ");
        serverUrl = Console.ReadLine() ?? "";

        Console.Write("What is your registered username on this server? (e.g. littlebigmolly) ");
        username = Console.ReadLine() ?? "";

        if (serverUrl == "" || username == "")
        {
            Logging.Message.Error("You must provide a valid server URL and/or username to continue.");
            return;
        }
        else if (!serverUrl.StartsWith("http://") && !serverUrl.StartsWith("https://"))
        {
            Logging.Message.Error("The server URL must start with http:// or https://.");
            return;
        }

        APIHttpClient = new HttpClient { BaseAddress = new Uri(serverUrl + "/api/v1/"), };
        AssetsHttpClient = new HttpClient { BaseAddress = new Uri(serverUrl + "/gameAssets/"), };

        DiscordClient.Initialize();
        DiscordClient.Logger = new ConsoleLogger { Level = LogLevel.Warning };

        DiscordClient.OnReady += (_, e) =>
        {
            Logging.Message.Info($"Connected to Discord Account {e.User.Username}#{e.User.Discriminator}.");
        };

        DiscordClient.OnPresenceUpdate += (_, e) =>
        {
            Logging.Message.Info($"{e.Presence}: Presence updated.");
        };

        while (true)
        {
            UpdatePresence(username);
            await Task.Delay(30000);
        }
    }

    public static class Tasks
    {
        public static async Task<Entities.User?> GetUser(string username)
        {
            if (UserCache.TryGetValue(username, out Entities.User? userObject) && userObject != null)
            {
                return userObject;
            }

            Logging.Message.Info($"Fetching user {username} from the server...");

            string userJson = await APIHttpClient.GetStringAsync("username/" + username);

            userObject = (Entities.User?)
                JsonSerializer.Deserialize(userJson, typeof(Entities.User));
            if (userObject == null)
                return null;

            UserCache.Add(username, userObject);
            return userObject;
        }

        public static async Task<Entities.UserStatus?> GetStatus(Entities.User? user)
        {
            Entities.UserStatus? userStatusObject = null;

            Logging.Message.Info($"Fetching status information for {username} from the server...");

            string userStatusJson = await APIHttpClient.GetStringAsync("user/" + user?.UserId + "/status");

            userStatusObject = (Entities.UserStatus?)
                JsonSerializer.Deserialize(userStatusJson, typeof(Entities.UserStatus));
            if (userStatusObject == null)
                return null;

            return userStatusObject;
        }

        public static async Task<Entities.Slot?> GetSlot(Entities.User? user, Entities.UserStatus? userStatus)
        {
            if (SlotCache.TryGetValue(Types.StateTypeExtensions.Id(
                userStatus?.CurrentRoom?.Slot?.SlotType,
                userStatus?.CurrentRoom?.Slot),
                out Entities.Slot? slotObject) && slotObject != null)
            {
                Logging.Message.Info($"Using cached slot information for {slotObject.SlotName} as {slotObject.SlotId}");
                return slotObject;
            }

            // Handle non-existent slots, this could be done better
            if (userStatus?.CurrentRoom?.Slot?.SlotType != Types.SlotType.User)
            {
                slotObject = new Entities.Slot()
                {
                    SlotName = Types.StateTypeExtensions.Slot(userStatus?.CurrentRoom?.Slot?.SlotType, slotObject),
                    SlotId = Types.StateTypeExtensions.Id(userStatus?.CurrentRoom?.Slot?.SlotType, userStatus?.CurrentRoom?.Slot),
                    IconHash = user?.IconHash,
                };

                Logging.Message.Info($"Caching a static slot for {slotObject.SlotName} as {slotObject.SlotId}");

                SlotCache.Add(slotObject.SlotId, slotObject);
                return slotObject;
            }

            Logging.Message.Info($"Fetching slot information for {userStatus?.CurrentRoom?.Slot?.SlotId} from the server...");

            string slotJson = await APIHttpClient.GetStringAsync("slot/" + userStatus?.CurrentRoom?.Slot?.SlotId);

            slotObject = (Entities.Slot?)
                JsonSerializer.Deserialize(slotJson, typeof(Entities.Slot));
            if (slotObject == null)
                return null;

            SlotCache.Add(userStatus?.CurrentRoom?.Slot?.SlotId ?? 0, slotObject);
            return slotObject;
        }
    }

    public static void UpdatePresence(string username)
    {
        Entities.User? user = Tasks.GetUser(username).Result;
        Entities.UserStatus? userStatus = Tasks.GetStatus(user).Result;
        Entities.Slot? slot = Tasks.GetSlot(user, userStatus).Result;
        Types.StatusType? statusType = userStatus?.StatusType;
        Types.SlotType? slotType = userStatus?.CurrentRoom?.Slot?.SlotType;

        DateTime lastLogin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(
            user?.LastLogin ?? 0
        );

        string[] StatusBuilder(Entities.Slot? slot, Entities.UserStatus? userStatus)
        {
            string Status = Types.StateTypeExtensions.Status(statusType, userStatus);
            string Slot = Types.StateTypeExtensions.Slot(slotType, slot);

            return new string[] { Status, Slot };
        }

        var presence = new RichPresence
        {
            Details = $"{StatusBuilder(slot, userStatus)[0]}",
            State = $"{StatusBuilder(slot, userStatus)[1]}",
            Assets = new Assets
            {
                LargeImageKey = serverUrl + "/gameAssets/" + slot?.IconHash,
                LargeImageText = slot?.SlotName,
                SmallImageKey = serverUrl + "/gameAssets/" + user?.YayHash,
                SmallImageText = user?.Username,
            },
            Timestamps = new Timestamps { Start = lastLogin, },
            Party = new Party
            {
                ID = $"room:{user?.UserId}:{userStatus?.CurrentRoom?.RoomId}",
                Size = userStatus?.CurrentRoom?.PlayerCount ?? 1,
                Max = 4,
            },
        };

        DiscordClient.SetPresence(presence);
        Logging.Message.Info($"{presence}: Sending presence update.");
    }
}

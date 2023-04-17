using DiscordRPC;
using DiscordRPC.Logging;
using LBPUnion.PLRPC.Extensions;
using LBPUnion.PLRPC.Logging;
using LBPUnion.PLRPC.Types;
using LBPUnion.PLRPC.Types.Entities;
using User = LBPUnion.PLRPC.Types.Entities.User;

namespace LBPUnion.PLRPC;

public class LighthouseClient
{
    private readonly IApiRepository apiRepository;
    private readonly DiscordRpcClient discordClient;
    private readonly SemaphoreSlim readySemaphore = new(0, 1);
    private readonly string serverUrl;
    private readonly string username;

    public LighthouseClient(string username, string serverUrl, IApiRepository apiRepository, DiscordRpcClient rpcClient)
    {
        this.username = username;
        this.serverUrl = serverUrl;
        this.apiRepository = apiRepository;

        this.discordClient = rpcClient;
        this.discordClient.Initialize();
        this.discordClient.Logger = new ConsoleLogger
        {
            Level = LogLevel.Warning,
        };

        this.discordClient.OnReady += (_, e) =>
        {
            Logger.Info($"Connected to Discord Account {e.User.Username}#{e.User.Discriminator}.");
            this.readySemaphore.Release();
        };
        this.discordClient.OnPresenceUpdate += (_, e) => Logger.Info($"{e.Presence}: Presence updated.");
    }

    private async void UpdatePresence()
    {
        User? user = await this.apiRepository.GetUser(this.username);
        if (user == null)
        {
            Logger.Warn("Failed to get user from the server.");
            return;
        }

        UserStatus? status = await this.apiRepository.GetStatus(user.UserId);
        if (status?.CurrentRoom?.Slot?.SlotId == null || status.CurrentRoom.PlayerIds == null)
        {
            Logger.Warn("Failed to get user status from the server.");
            return;
        }

        SlotType slotType = status.CurrentRoom.Slot.SlotType;
        Slot? slot;

        if (slotType is SlotType.User)
        {
            slot = await this.apiRepository.GetSlot(status.CurrentRoom.Slot.SlotId);
            if (slot == null)
            {
                Logger.Warn("Failed to get user's current level from the server.");
                return;
            }
        }
        else
        {
            string iconHash = slotType switch
            {
                SlotType.Pod => "9c412649a07a8cb678a2a25214ed981001dd08ca",
                SlotType.Moon => "a891bbcf9ad3518b80c210813cce8ed292ed4c62",
                SlotType.Developer => "2976e45d66b183f6d3242eaf01236d231766295f",
                SlotType.DLC => "7d3df5ce61ca90a80f600452cd3445b7a775d47e",
                _ => "e6bb64f5f280ce07fdcf4c63e25fa8296c73ec29",
            };

            slot = new Slot
            {
                IconHash = iconHash,
            };
        }

        int playersInRoom = status.CurrentRoom.PlayerIds.Length;
        int roomId = status.CurrentRoom.RoomId;
        int userId = user.UserId;

        string details = slotType switch
        {
            SlotType.User => $"{slot.Name}",
            SlotType.Pod => "Dwelling in the Pod",
            SlotType.Moon => "Creating on the Moon",
            SlotType.Developer => "Playing a Story Level",
            SlotType.DLC => "Playing a DLC Level",
            _ => "(っ◔◡◔)っ ❤",
        };

        string userStatus = status.StatusType switch
        {
            StatusType.Online => $"{status.CurrentVersion.ToPrettyString()}",
            StatusType.Offline => "",
            _ => "Unknown State",
        };

        RichPresence newPresence = new()
        {
            Details = details,
            State = userStatus,
            Assets = new Assets
            {
                LargeImageKey = this.serverUrl + "/gameAssets/" + slot.IconHash,
                LargeImageText = slot.Name,
                SmallImageKey = this.serverUrl + "/gameAssets/" + user.YayHash,
                SmallImageText = user.Username,
            },
            Timestamps = new Timestamps
            {
                Start = Timestamps.FromUnixMilliseconds((ulong)user.LastLogin),
            },
            Party = new Party
            {
                ID = $"room:{userId}:{roomId}",
                Size = playersInRoom,
                Max = 4,
            },
        };
        this.discordClient.SetPresence(newPresence);
        Logger.Info($"{newPresence}: Sending presence update.");
    }

    public async Task StartUpdateLoop()
    {
        await this.readySemaphore.WaitAsync();
        this.readySemaphore.Dispose();
        while (true)
        {
            try
            {
                this.UpdatePresence();
            }
            catch (Exception exception)
            {
                this.discordClient.Dispose();
                Logger.LogException(exception);
                Thread.Sleep(10000);
                Environment.Exit(1);
            }

            await Task.Delay(30000);
        }
    }
}
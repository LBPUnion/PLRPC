using DiscordRPC;
using LBPUnion.PLRPC.Extensions;
using LBPUnion.PLRPC.Types.Entities;
using LBPUnion.PLRPC.Types.Enums;
using LBPUnion.PLRPC.Types.Interfaces;
using LBPUnion.PLRPC.Types.Logging;
using User = LBPUnion.PLRPC.Types.Entities.User;

namespace LBPUnion.PLRPC;

public class LighthouseClient
{
    private readonly IApiRepository apiRepository;
    private readonly DiscordRpcClient discordRpcClient;
    private readonly Logger logger;
    private readonly SemaphoreSlim readySemaphore = new(0, 1);

    private readonly string serverUrl;
    private readonly string username;

    public LighthouseClient(string username, string serverUrl, IApiRepository apiRepository, DiscordRpcClient discordRpcClient, Logger logger)
    {
        this.username = username;
        this.serverUrl = serverUrl;
        this.apiRepository = apiRepository;

        this.discordRpcClient = discordRpcClient;
        this.discordRpcClient.Initialize();

        this.logger = logger;

        this.discordRpcClient.OnReady += (_, _) => this.readySemaphore.Release();

        this.discordRpcClient.OnReady += (_, _) =>
            this.logger.Information("Successfully established ready connection", LogArea.RichPresence);

        this.discordRpcClient.OnConnectionEstablished += (_, e) =>
            this.logger.Information($"Successfully acquired the lock on RPC ({e.ConnectedPipe})", LogArea.RichPresence);

        this.discordRpcClient.OnConnectionFailed += (_, e) =>
            this.logger.Warning($"Failed to acquire the lock on RPC ({e.FailedPipe})", LogArea.RichPresence);

        this.discordRpcClient.OnPresenceUpdate += (_, e) =>
            this.logger.Information($"Updated client presence ({e.Presence.Party.ID})", LogArea.RichPresence);
    }

    private async Task UpdatePresence()
    {
        User? user = await this.apiRepository.GetUser(this.username);
        if (user == null || user.PermissionLevel == PermissionLevel.Banned)
        {
            this.logger.Warning("Failed to get user from the server", LogArea.ApiRepositoryImpl);
            return;
        }

        UserStatus? userStatus = await this.apiRepository.GetStatus(user.UserId);
        if (userStatus?.CurrentRoom?.Slot?.SlotId == null || userStatus.CurrentRoom.PlayerIds == null)
        {
            this.logger.Warning("Failed to get user status from the server", LogArea.ApiRepositoryImpl);
            return;
        }

        SlotType slotType = userStatus.CurrentRoom.Slot.SlotType;
        Slot? slot;

        if (slotType == SlotType.User)
        {
            slot = await this.apiRepository.GetSlot(userStatus.CurrentRoom.Slot.SlotId);
            if (slot == null)
            {
                this.logger.Warning("Failed to get user's current level from the server", LogArea.ApiRepositoryImpl);
                return;
            }
        }
        else
        {
            string iconHash = slotType switch
            {
                SlotType.Pod => "9c412649a07a8cb678a2a25214ed981001dd08ca",
                SlotType.Moon => "a891bbcf9ad3518b80c210813cce8ed292ed4c62",
                SlotType.RemoteMoon => "a891bbcf9ad3518b80c210813cce8ed292ed4c62",
                SlotType.Developer => "7d3df5ce61ca90a80f600452cd3445b7a775d47e",
                SlotType.DeveloperAdventure => "7d3df5ce61ca90a80f600452cd3445b7a775d47e",
                SlotType.DlcLevel => "2976e45d66b183f6d3242eaf01236d231766295f",
                SlotType.Unknown => "e6bb64f5f280ce07fdcf4c63e25fa8296c73ec29",
                _ => "e6bb64f5f280ce07fdcf4c63e25fa8296c73ec29",
            };

            slot = new Slot
            {
                IconHash = iconHash,
            };
        }

        int playersInRoom = userStatus.CurrentRoom.PlayerIds.Length;
        int roomId = userStatus.CurrentRoom.RoomId;
        int userId = user.UserId;

        string details = slotType switch
        {
            SlotType.User => $"{slot.Name}",
            SlotType.Pod => "Dwelling in the Pod",
            SlotType.Moon => "Creating on the Moon",
            SlotType.RemoteMoon => "Creating on a Remote Moon",
            SlotType.Developer => "Playing a Story Level",
            SlotType.DeveloperAdventure => "Playing an Adventure Level",
            SlotType.DlcLevel => "Playing a DLC Level",
            SlotType.Unknown => "Exploring the Imagisphere",
            _ => "Exploring the Imagisphere",
        };

        string status = userStatus.StatusType switch
        {
            StatusType.Online => $"{userStatus.CurrentVersion.ToPrettyString()}",
            StatusType.Offline => "",
            _ => "Unknown State",
        };

        RichPresence newPresence = new()
        {
            Details = details,
            State = status,
            Assets = new Assets
            {
                LargeImageKey = this.serverUrl + "/gameAssets/" + slot.IconHash,
                LargeImageText = slot.Name,
                SmallImageKey = this.serverUrl + "/gameAssets/" + user.YayHash,
                SmallImageText = user.Username + user.PermissionLevel.ToPrettyString(),
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
            Buttons = new[]
            {
                new Button
                {
                    Label = $"View {user.Username}'s Profile",
                    Url = $"{this.serverUrl}/user/{userId}",
                },
            },
        };

        this.logger.Information($"Updating client presence ({newPresence.Party.ID})", LogArea.RichPresence);

        this.discordRpcClient.SetPresence(newPresence);
    }

    public async Task StartUpdateLoop()
    {
        await this.readySemaphore.WaitAsync();
        this.readySemaphore.Dispose();

        while (true)
        {
            try
            {
                await this.UpdatePresence();
            }
            catch (Exception)
            {
                this.discordRpcClient.ClearPresence();
                this.discordRpcClient.Dispose();
                return;
            }
            await Task.Delay(30000);
        }
    }
}
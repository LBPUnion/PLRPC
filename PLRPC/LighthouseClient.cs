using DiscordRPC;
using LBPUnion.PLRPC.Extensions;
using LBPUnion.PLRPC.Helpers;
using LBPUnion.PLRPC.Types.Configuration;
using LBPUnion.PLRPC.Types.Entities;
using LBPUnion.PLRPC.Types.Interfaces;
using LBPUnion.PLRPC.Types.Logging;
using User = LBPUnion.PLRPC.Types.Entities.User;

namespace LBPUnion.PLRPC;

public class LighthouseClient
{
    private readonly string username;
    private readonly string serverUrl;

    private readonly RemoteConfiguration remoteConfiguration;
    private readonly IApiRepository apiRepository;
    private readonly DiscordRpcClient discordRpcClient;
    private readonly Logger logger;
    private readonly SemaphoreSlim readySemaphore = new(0, 1);

    public LighthouseClient(string username, string serverUrl, RemoteConfiguration remoteConfiguration, IApiRepository apiRepository, DiscordRpcClient discordRpcClient, Logger logger)
    {
        this.username = username;
        this.serverUrl = serverUrl;

        this.remoteConfiguration = remoteConfiguration;
        this.apiRepository = apiRepository;

        this.logger = logger;

        this.discordRpcClient = discordRpcClient;
        this.discordRpcClient.Initialize();

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
        if (user == null)
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
            string? iconHash = slotType switch
            {
                SlotType.Pod => this.remoteConfiguration.Assets.PodAsset,
                SlotType.Moon => this.remoteConfiguration.Assets.MoonAsset,
                SlotType.RemoteMoon => this.remoteConfiguration.Assets.RemoteMoonAsset,
                SlotType.Developer => this.remoteConfiguration.Assets.DeveloperAsset,
                SlotType.DeveloperAdventure => this.remoteConfiguration.Assets.DeveloperAdventureAsset,
                SlotType.DlcLevel => this.remoteConfiguration.Assets.DlcAsset,
                SlotType.Unknown => this.remoteConfiguration.Assets.FallbackAsset,
                _ => this.remoteConfiguration.Assets.FallbackAsset,
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

        string state = userStatus.StatusType switch
        {
            StatusType.Online => $"{userStatus.CurrentVersion.ToPrettyString()}",
            StatusType.Offline => "",
            _ => "Unknown State",
        };

        RichPresence newPresence = new()
        {
            Details = details,
            State = state,
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
                ID = $"PLRPC:{CryptoHelper.Sha1Hash(this.serverUrl)[..7]}:{userId}:{roomId}",
                Size = playersInRoom,
                Max = 4,
            },
            Buttons = new[]
            {
                new Button
                {
                    Label = $"View {user.Username}'s Profile",
                    Url = $"{this.serverUrl}/user/{(this.remoteConfiguration.UsernameType == UsernameType.Integer ? userId : user.Username)}",
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
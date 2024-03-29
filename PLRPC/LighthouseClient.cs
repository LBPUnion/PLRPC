﻿using DiscordRPC;
using LBPUnion.PLRPC.Extensions;
using LBPUnion.PLRPC.Helpers;
using LBPUnion.PLRPC.Types.Configuration;
using LBPUnion.PLRPC.Types.Entities;
using LBPUnion.PLRPC.Types.Enums;
using LBPUnion.PLRPC.Types.Interfaces;
using LBPUnion.PLRPC.Types.Logging;
using User = LBPUnion.PLRPC.Types.Entities.User;

namespace LBPUnion.PLRPC;

public class LighthouseClient
{
    private readonly string username;
    private readonly string serverUrl;
    
    private readonly ILighthouseApi lighthouseApi;
    private readonly RemoteConfiguration remoteConfiguration;
    private readonly DiscordRpcClient discordRpcClient;
    private readonly Logger logger;

    private readonly SemaphoreSlim readySemaphore = new(0, 1);

    public LighthouseClient(string username, string serverUrl, ILighthouseApi lighthouseApi, RemoteConfiguration remoteConfiguration, DiscordRpcClient discordRpcClient, Logger logger)
    {
        this.username = username;
        this.serverUrl = serverUrl;

        this.lighthouseApi = lighthouseApi;
        this.remoteConfiguration = remoteConfiguration;

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

    // ReSharper disable once CognitiveComplexity
    private async Task UpdatePresence()
    {
        User? user = await this.lighthouseApi.GetUser(this.username);
        if (user == null)
        {
            this.logger.Error("Failed to get user from the server", LogArea.LighthouseApi);
            return;
        }

        UserStatus? userStatus = await this.lighthouseApi.GetStatus(user.UserId);
        if (userStatus?.CurrentRoom?.Slot?.SlotId == null || userStatus.CurrentRoom.PlayerIds == null)
        {
            this.logger.Error("Failed to get user status from the server", LogArea.LighthouseApi);
            return;
        }

        SlotType slotType = userStatus.CurrentRoom.Slot.SlotType;
        Slot? slot;

        if (slotType == SlotType.User)
        {
            slot = await this.lighthouseApi.GetSlot(userStatus.CurrentRoom.Slot.SlotId);
            if (slot == null)
            {
                this.logger.Error("Failed to get user's current level from the server", LogArea.LighthouseApi);
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
            if (iconHash == null) this.logger.Warning($"Server asset for {slotType.ToString()} doesn't exist", LogArea.Configuration);

            slot = new Slot
            {
                IconHash = iconHash,
            };
        }

        bool useApplicationAssets = this.remoteConfiguration.Assets.UseApplicationAssets;

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

        string? largeImageKey = useApplicationAssets ? slot.IconHash : this.serverUrl + "/gameAssets/" + slot.IconHash;
        string? smallImageKey = useApplicationAssets ? null : this.serverUrl + "/gameAssets/" + user.YayHash;

        if (useApplicationAssets)
        {
            if (largeImageKey == null)
                this.logger.Warning($"Application asset for {slotType.ToString()} doesn't exist", LogArea.Configuration);
            if (smallImageKey == null)
                this.logger.Information("Server prefers application assets, small image will be hidden", LogArea.Configuration);
        }

        RichPresence newPresence = new()
        {
            Details = details,
            State = state,
            Assets = new Assets
            {
                LargeImageKey = largeImageKey,
                LargeImageText = slot.Name,
                SmallImageKey = smallImageKey,
                SmallImageText = user.Username + user.PermissionLevel.ToPrettyString(),
            },
            Timestamps = new Timestamps
            {
                Start = Timestamps.FromUnixMilliseconds((ulong)user.LastLogin),
            },
            Party = new Party
            {
                ID = $"{this.remoteConfiguration.PartyIdPrefix}:{CryptoHelper.Sha1Hash(this.serverUrl)[..7]}:{userId}:{roomId}",
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
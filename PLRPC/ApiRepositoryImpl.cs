using System.Text.Json;
using LBPUnion.PLRPC.Types.Entities;
using LBPUnion.PLRPC.Types.Enums;
using LBPUnion.PLRPC.Types.Interfaces;

namespace LBPUnion.PLRPC;

public class ApiRepositoryImpl : IApiRepository
{
    private readonly TimeSpan cacheExpirationTime;
    private readonly HttpClient httpClient;

    private readonly Dictionary<int, (Slot, long)> slotCache = new();
    private readonly Dictionary<string, (User, long)> userCache = new();
    // private readonly Dictionary<int, (UserStatus, long)> userStatusCache = new();

    public ApiRepositoryImpl(HttpClient httpClient, TimeSpan cacheExpirationTime)
    {
        this.httpClient = httpClient;
        this.cacheExpirationTime = cacheExpirationTime;
    }

    private static long TimestampMillis => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public async Task<User?> GetUser(string username)
    {
        if (this.GetFromCache(this.userCache, username, out User? cachedUser)) return cachedUser;

        HttpResponseMessage userReq = await this.httpClient.GetAsync($"username/{username}");
        if (!userReq.IsSuccessStatusCode) return null;

        User? user = JsonSerializer.Deserialize<User>(await userReq.Content.ReadAsStringAsync());
        if (user == null) return null;

        this.userCache.TryAdd(username, (user, TimestampMillis));
        return user;
    }

    public async Task<Slot?> GetSlot(int slotId)
    {
        if (this.GetFromCache(this.slotCache, slotId, out Slot? cachedSlot)) return cachedSlot;

        HttpResponseMessage slotReq = await this.httpClient.GetAsync($"slot/{slotId}");
        if (!slotReq.IsSuccessStatusCode) return null;

        Slot? slot = JsonSerializer.Deserialize<Slot>(await slotReq.Content.ReadAsStringAsync());
        if (slot == null) return null;

        this.slotCache.TryAdd(slotId, (slot, TimestampMillis));
        return slot;
    }

    public async Task<UserStatus?> GetStatus(int userId)
    {
        /*
         * User status cache is disabled for now, causes issue where old status is returned
         * Will need to revisit later or remove all together.
         */

        // if (this.GetFromCache(this.userStatusCache, userId, out UserStatus? cachedUserStatus)) return cachedUserStatus;

        HttpResponseMessage userStatusReq = await this.httpClient.GetAsync($"user/{userId}/status");
        if (!userStatusReq.IsSuccessStatusCode) return null;

        UserStatus? userStatus = JsonSerializer.Deserialize<UserStatus>(await userStatusReq.Content.ReadAsStringAsync());
        if (userStatus == null) return null;

        /*
         * LBP1 doesn't support rooms, so we have to create a fake one if the user is playing it
         * This is a bit of a hacksaw solution but it works, will have to revisit later
         */
        if (userStatus.CurrentVersion == GameVersion.LittleBigPlanet1)
        {
            userStatus.CurrentRoom = new Room
            {
                RoomId = 0,
                PlayerIds = new[]
                {
                    userId,
                },
                Slot = new RoomSlot
                {
                    SlotId = 0,
                    SlotType = SlotType.Unknown,
                },
            };
        }

        // this.userStatusCache.TryAdd(userId, (userStatus, TimestampMillis));
        return userStatus;
    }

    private bool GetFromCache<T1, T2>(IReadOnlyDictionary<T1, (T2, long)> cache, T1 key, out T2? val) where T1 : notnull
    {
        val = default;
        if (!cache.TryGetValue(key, out (T2, long) entry)) return false;

        if (entry.Item2 + this.cacheExpirationTime.Milliseconds > TimestampMillis) return false;

        val = entry.Item1;
        return true;
    }
}
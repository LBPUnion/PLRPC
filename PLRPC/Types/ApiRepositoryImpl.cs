using System.Text.Json;
using LBPUnion.PLRPC.Types.Entities;

namespace LBPUnion.PLRPC.Types;

public class ApiRepositoryImpl : IApiRepository
{
    private readonly int cacheExpirationTimeMs;

    private readonly HttpClient httpClient;
    private readonly Dictionary<int, (Slot, long)> slotCache = new();
    private readonly Dictionary<string, (User, long)> userCache = new();
    private readonly Dictionary<int, (UserStatus, long)> userStatusCache = new();

    public ApiRepositoryImpl(HttpClient httpClient, int cacheExpirationTimeMs)
    {
        this.httpClient = httpClient;
        this.cacheExpirationTimeMs = cacheExpirationTimeMs;
    }

    private static long TimestampMillis => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public async Task<User?> GetUser(string username)
    {
        if (this.GetFromCache(this.userCache, username, out User? cachedUser)) return cachedUser;

        string userJson = await this.httpClient.GetStringAsync($"username/{username}");

        User? user = JsonSerializer.Deserialize<User>(userJson);
        if (user == null) return null;

        this.userCache.TryAdd(username, (user, TimestampMillis));
        return user;
    }

    public async Task<UserStatus?> GetStatus(int userId)
    {
        if (this.GetFromCache(this.userStatusCache, userId, out UserStatus? cachedUserStatus)) return cachedUserStatus;

        string userStatusJson = await this.httpClient.GetStringAsync($"user/{userId}/status");

        UserStatus? userStatus = JsonSerializer.Deserialize<UserStatus>(userStatusJson);
        if (userStatus == null) return null;

        this.userStatusCache.TryAdd(userId, (userStatus, TimestampMillis));
        return userStatus;
    }

    public async Task<Slot?> GetSlot(int slotId)
    {
        if (this.GetFromCache(this.slotCache, slotId, out Slot? cachedSlot)) return cachedSlot;

        string slotJson = await this.httpClient.GetStringAsync($"slot/{slotId}");

        Slot? slot = JsonSerializer.Deserialize<Slot>(slotJson);
        if (slot == null) return null;

        this.slotCache.TryAdd(slotId, (slot, TimestampMillis));
        return slot;
    }

    private bool GetFromCache<T1, T2>(IReadOnlyDictionary<T1, (T2, long)> cache, T1 key, out T2? val) where T1 : notnull
    {
        val = default;
        if (!cache.TryGetValue(key, out (T2, long) entry)) return false;

        if (entry.Item2 + this.cacheExpirationTimeMs > TimestampMillis) return false;

        val = entry.Item1;
        return true;
    }
}
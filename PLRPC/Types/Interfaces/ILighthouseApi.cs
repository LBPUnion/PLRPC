using LBPUnion.PLRPC.Types.Entities;

namespace LBPUnion.PLRPC.Types.Interfaces;

public interface ILighthouseApi
{
    public Task<User?> GetUser(string username);
    public Task<Slot?> GetSlot(int slotId);
    public Task<UserStatus?> GetStatus(int userId);
}
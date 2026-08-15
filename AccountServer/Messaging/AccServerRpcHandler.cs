using System;
using System.Threading.Tasks;
using Common;
using Common.Database;
using Common.Database.Models;
using Common.Game;
using Common.Messaging;
using Common.Structs;
using Common.Utilities;
using StreamJsonRpc;

namespace AccountServer.Messaging;

public class AccServerRpcHandler : IAccountServerHandler {
    private static readonly Logger _log = new Logger(typeof(AccServerRpcHandler));

    public Guid ServerId { get; set; }

    private IGameServerRpc _proxy;

    public void Attach(IGameServerRpc proxy) {
        _proxy = proxy;
    }

    public void Close() {
        IpcServer.Clients.TryRemove(ServerId, out _);
        DbClient.Accounts.UpdateMany( // Release locks acquired by this GameServer instance
            acc => new Account { LockOwner = Guid.Empty },
            acc => acc.LockOwner == ServerId);
    }
    
    public Task GameServerConnected(Guid gameServerId) {
        if (!IpcServer.Clients.TryAdd(gameServerId, _proxy))
            throw new InvalidOperationException($"Failed to add GameServer to dictionary: {gameServerId}");
        ServerId = gameServerId;
        
        _log.Info($"[RPC] GameServer ({gameServerId}) connected");
        return Task.CompletedTask;
    }

    public async Task<GameInfoDto?> GetUserInfo(string name, int accountId) {
        return await _proxy.GetUserInfo(name, accountId);
    }
}
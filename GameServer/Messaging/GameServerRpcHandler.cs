using Common;
using Common.Messaging;
using Common.Structs;
using Common.Utilities;
using GameServer.Game;

namespace GameServer.Messaging;

public class GameServerRpcHandler : IGameServerRpc {
    private static readonly Logger _log = new Logger(typeof(GameServerRpcHandler));
    
    public Task<bool> GlobalAnnouncement(string from, string message) {
        _log.Info($"[RPC](GlobalAnnouncement) {from}: {message}");
        return Task.FromResult(true);
    }
    
    public Task<ServerInfo> GetGameServer() {
        return Task.FromResult(new ServerInfo(Program.Guid, ServerType.GameServer, GameLogic.WorldTime.TotalElapsedMs, RealmManager.Users.Count));
    }
}
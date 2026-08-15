using Common;
using Common.Game;
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
    
    public Task<GameInfoDto?> GetUserInfo(string name, int accountId) {
        var target = RealmManager.Users.Values.FirstOrDefault(c => c.GameInfo.Account.Id == accountId || c.GameInfo.Account.Name == name);
        return Task.FromResult(target?.GameInfo.Data);
    }
}
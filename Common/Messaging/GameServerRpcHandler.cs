using System.Threading.Tasks;
using Common.Utilities;

namespace Common.Messaging;

public class GameServerRpcHandler : IGameServerRpc {
    private static readonly Logger _log = new Logger(typeof(GameServerRpcHandler));
    
    public Task<bool> GlobalAnnouncement(string from, string message) {
        _log.Info($"[RPC](GlobalAnnouncement) {from}: {message}");
        return Task.FromResult(true);
    }
    
    public Task<GameServerStatus> GetGameServerStatus() {
        return Task.FromResult(new GameServerStatus(0, 5, 0));
    }
}
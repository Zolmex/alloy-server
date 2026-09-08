using Common.Resources.Config;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Network;

namespace GameServer.Game.Worlds.Logic;

public class Vault : World {
    private static readonly Dictionary<int, Vault> _vaults = [];
    
    public Vault(int id, int mapId, WorldConfig config)
        : base(id, mapId, config) {
    }

    public override World GetInstance(User user) {
        if (user.Session.Account == null)
            return null;

        if (!_vaults.TryGetValue(user.Session.Account.Id, out var ret) || ret.Deleted) {
            ret = _vaults[user.Session.Account.Id] = new Vault(0, MapId, Config);
            RealmManager.AddWorld(ret);
        }

        return ret;
    }
}
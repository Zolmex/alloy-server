using Common.Resources.Config;
using Common.Resources.World;
using Common.Utilities;

namespace GameServer.Game.Worlds.Logic;

public class Realm : World {
    public Realm()
        : base(0, -1, WorldLibrary.WorldConfigs["Realm"]) {
        DisplayName = RealmManager.GetNewRealmName();
    }
}
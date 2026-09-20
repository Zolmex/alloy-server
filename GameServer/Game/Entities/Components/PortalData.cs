using System.Reflection;
using Common.Utilities;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct PortalData {
    public bool DisplayPlayerCount;
    public bool Locked;
    public int WorldId;

    public void LinkWorld(World world) {
        WorldId = world.Id;
    }

    public World GetInstance(User user) {
        if (WorldId == 0)
            return null;
        return RealmManager.Worlds[WorldId].GetInstance(user);
    }
}
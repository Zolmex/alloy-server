using System.Reflection;
using Common.Utilities;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct PortalData {
    private static readonly Dictionary<string, Type> _worldTypes = [];
    private static readonly Logger _log = new(typeof(PortalData));

    static PortalData() {
        var asm = Assembly.GetExecutingAssembly();
        foreach (var type in asm.GetTypes()) {
            if (type == typeof(World) || !type.IsSubclassOf(typeof(World)))
                continue;

            _worldTypes[type.Name] = type;
        }
    }
    
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
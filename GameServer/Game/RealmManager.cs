using System.Collections.Immutable;
using System.Diagnostics;
using Common.Utilities;
using GameServer.Game.Worlds;
using GameServer.Game.Worlds.Logic;

namespace GameServer.Game;

public class RealmManager {
    private static readonly Logger _log = new(typeof(RealmManager));

    public static ImmutableDictionary<int, World> Worlds = ImmutableDictionary.Create<int, World>();
    
    public static void Init() {
        AddWorld(World.NEXUS_ID, 0, new Nexus());
        
        _log.Info("Realm Manager initialized.");
    }

    public static void AddWorld(int id, int mapId, World world) {
        Worlds = Worlds.Add(id, world);
        world.Load(mapId);
    }
}
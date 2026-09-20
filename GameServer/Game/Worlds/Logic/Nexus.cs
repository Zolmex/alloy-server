using Common.Resources.Config;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Old;
using GameServer.Game.Entities.Old.Extensions;

namespace GameServer.Game.Worlds.Logic;

public class Nexus : World {

    private static readonly Logger _log = new (typeof(Nexus));
    
    private readonly HashSet<MapTileData> _realmPortals = new();
    private readonly List<MapTileData> _realmPortalTiles = new();
    
    public Nexus() : base(NEXUS_ID, 0, WorldLibrary.WorldConfigs["Nexus"]) {
        foreach (var pos in Map.Regions[TileRegion.Realm_Portals])
            _realmPortalTiles.Add(Map[pos.X, pos.Y]);

        var realmCount = GameServerConfig.Config.RealmCount;
        if (realmCount == 0)
            return;
        
        for (var i = 0; i < realmCount; i++)
        {
            if (_realmPortals.Count >= _realmPortalTiles.Count)
                break;
        
            AddRealmPortal();
        }
    }

    public void AddRealmPortal() {
        if (_realmPortals.Count >= _realmPortalTiles.Count) {
            _log.Error($"All realm portal regions have been occupied ({_realmPortalTiles.Count}).");
            return;
        }
        
        var portal = EnterWorld(XmlLibrary.ObjectDescs[0x0704]);
        
        MapTileData tile = null; // Select a random realm portal tile
        while (tile == null || _realmPortals.Contains(tile))
            tile = _realmPortalTiles.RandomElement();

        ref var portalPos = ref Ecs.Get<Position>(portal);
        portalPos.Move(tile.X, tile.Y);
        _realmPortals.Add(tile);

        var realm = new Realm();
        RealmManager.AddWorld(realm);
        
        ref var portalData = ref Ecs.Get<PortalData>(portal);
        portalData.DisplayPlayerCount = true;
        portalData.LinkWorld(realm);
    }
}
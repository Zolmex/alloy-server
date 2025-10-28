#region

using Common.Resources.Config;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Entities;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Worlds
{
    public class Nexus : World
    {
        private readonly HashSet<WorldTile> _realmPortals = new();
        private readonly List<WorldTile> _realmPortalTiles = new();

        public Dictionary<Portal, string> AllPortals = new();

        public Nexus() : base(NEXUS, 0, -1)
        { }

        public override void Initialize()
        {
            base.Initialize();

            // Load all tiles with RealmPortals region
            foreach (var pos in Map.Regions[TileRegion.Realm_Portals])
                _realmPortalTiles.Add(Map[(int)pos.X, (int)pos.Y]);

            var realmCount = GameServerConfig.Config.RealmCount;
            if (realmCount == 0)
                return;

            // Spawn realm portals
            for (var i = 0; i < realmCount; i++)
            {
                if (_realmPortals.Count >= _realmPortalTiles.Count)
                    break;

                var portal = new Portal(0x0704, true);
                AddRealmPortal(portal);
            }
        }

        // My take on a PortalMonitor. Just.. not in a separate class B).
        public void AddPortal(World world)
        {
            var attach = world is Realm;
            var type = attach ? 0x0704 : 0x0703;
            var portal = new Portal((ushort)type, attach);
            AddRealmPortal(portal);
        }

        public void RemovePortal(string name) // Primarily used for realm portals.
        {
            foreach (var portal in AllPortals.Keys)
            {
                var portalWorld = portal.PortalWorld;
                if (portalWorld.DisplayName == name)
                {
                    portal.TryLeaveWorld();
                    AllPortals.Remove(portal);
                }
            }
        }

        public void RemovePortal(World target)
        {
            foreach (var portal in AllPortals.Keys)
            {
                var portalWorld = portal.PortalWorld;
                if (portalWorld == target)
                {
                    portal.TryLeaveWorld();
                    AllPortals.Remove(portal);
                }
            }
        }

        private void AddRealmPortal(Portal portal)
        {
            WorldTile tile = null; // Select a random realm portal tile
            while (tile == null || _realmPortals.Contains(tile))
                tile = _realmPortalTiles.RandomElement();

            portal.Move(tile.X + 0.5f, tile.Y + 0.5f);
            portal.EnterWorld(this);
            _realmPortals.Add(tile);
            AllPortals.Add(portal, portal.Name);
        }
    }
}
using System;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct PortalData : IEntityIdentifiable, IDisposable {
    public EntityId Id { get; set; }

    public World WorldLink;
    public bool DisplayPlayerCount;
    public bool Disabled;
    
    private readonly World _world;
    
    public PortalData(World world, ref Entity en) {
        Id = en.Id;
        _world = world;
    }

    public void Init(World worldLink) {
        RealmManager.AddWorld(worldLink);
        LinkTo(worldLink);
    }

    public void LinkTo(World worldLink) {
        WorldLink = worldLink;
    }

    public void Tick(ref RealmTime time) {
        if (WorldLink == null)
            return;

        if (WorldLink.Deleted) {
            _world.LeaveWorld(Id);
        }
        
        if (DisplayPlayerCount) {
            ref var stats = ref _world.EntityStats.Get(Id);
            stats.Set(StatType.Name, $"{WorldLink.DisplayName} ({WorldLink.Users.Count}/{WorldLink.Config.MaxPlayers})");
        }
    }
    
    public void Dispose() {
        
    }
}
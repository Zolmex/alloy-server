using Arch.Core;
using Arch.System;
using Common;
using Common.Game;
using GameServer.Game.Entities.Components;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Systems;

public partial class PortalSystem(World world) : BaseSystem<World, RealmTime>(world) {

    public void Tick(ref RealmTime time) {
        ProcessQuery(World.Ecs);
    }

    [Query]
    public void Process(Entity en, ref Stats stats, ref PortalData portal) {
        var world = portal.GetInstance(null);
        if (world == null)
            return;
        
        if (world.Deleted) {
            world.LeaveWorld(en);
            return;
        }

        stats.Set(StatType.PortalUsable, portal.Locked ? 0 : 1); // Idk if it works like this
        if (portal.DisplayPlayerCount) {
            stats.Set(StatType.Name, $"{world.DisplayName} ({world.Users.Count}/{world.Config.MaxPlayers})");
        }
    }
}
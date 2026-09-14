using Arch.Core;
using Arch.System;
using Collections.Pooled;
using Common.Game;
using GameServer.Game.Entities.Events;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Systems;

public partial class EventSystem(World world) : BaseSystem<World, RealmTime>(world) { // Invoke, Subscribe and Unsubscribe are source-generated methods

    private readonly PooledDictionary<Entity, EventRouter> _eventRouters = [];
}
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Events;

public record struct DeathEvent(World World, EntityId HostId);
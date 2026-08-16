using Common.Utilities.Collections;
using GameServer.Game.Entities.Old.Components;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Old.Events;

public record struct DeathEvent(World World, EntityId HostId);
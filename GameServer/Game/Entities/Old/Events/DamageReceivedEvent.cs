using Common.Utilities.Collections;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Events;

public record struct DamageReceivedEvent(World world, EntityId HostId, int Damage);
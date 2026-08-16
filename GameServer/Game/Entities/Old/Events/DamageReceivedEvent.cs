using Common.Utilities.Collections;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Old.Events;

public record struct DamageReceivedEvent(World world, EntityId HostId, int Damage);
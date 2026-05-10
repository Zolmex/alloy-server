using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Events;

public record struct DamageReceivedEvent(World world, int HostId, int Damage);
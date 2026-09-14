using Arch.Bus;
using Arch.Core;
using Common;
using GameServer.Game.Entities.Components;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Events;

public readonly record struct DamageReceivedEvent(Entity Entity, int Damage);
using Arch.Core;
using Common;
using GameServer.Game.Entities.Components;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Events;

public readonly record struct DeathEvent(Entity Entity);
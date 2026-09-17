using Arch.Core;
using Common;
using Common.Game;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Components;

public record struct Combat(int DamageReceived) {}
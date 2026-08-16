using Common.Utilities;

namespace GameServer.Game.Entities.Components;

public record struct Flags(BitMask256 Mask);
using Common.Structs;

namespace Common.Game;

public readonly record struct SessionDto(int AccountId, int WorldId, string WorldName, WorldPosData Position);
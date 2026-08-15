using Common.Structs;

namespace Common.Game;

public readonly record struct GameInfoDto(int AccountId, int WorldId, string WorldName, WorldPosData Position);
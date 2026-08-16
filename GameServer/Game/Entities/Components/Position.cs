using Common.Resources.World;
using Common.Structs;

namespace GameServer.Game.Entities.Components;

public record struct Position(
    WorldPosData Pos,
    WorldPosData PrevPos,
    WorldPosData SpawnPos,
    MapTileData Tile
    );
using Common.Resources.World;
using Common.Structs;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public record struct Position(
    WorldPosData Pos,
    WorldPosData PrevPos,
    WorldPosData SpawnPos,
    MapTileData Tile
) {
    public void Init(World world, WorldPosData spawn) {
        Pos = spawn;
        PrevPos = spawn;
        SpawnPos = spawn;
        Tile = world.Map[(int)Pos.X, (int)Pos.Y];
    }
}
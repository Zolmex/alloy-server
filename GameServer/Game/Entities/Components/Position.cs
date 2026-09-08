using System.Numerics;
using Arch.Core;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using GameServer.Utilities;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Components;

public record struct Position(
    WorldPosData Pos,
    WorldPosData PrevPos,
    WorldPosData SpawnPos
) {
    public bool PositionUpdate;
    private bool _spawnSet;
    
    public void Init(World world, WorldPosData spawn) {
        Pos = spawn;
        PrevPos = spawn;
        SpawnPos = spawn;
    }
    
    public void Move(in Vector2 vec) {
        Move(vec.X, vec.Y);
    }
    
    public void Move(float newX, float newY) {
        Pos = new WorldPosData(newX, newY);
        PositionUpdate = true;
        if (!_spawnSet) {
            _spawnSet = true;
            SpawnPos = Pos;
        }
    }
    
    
}
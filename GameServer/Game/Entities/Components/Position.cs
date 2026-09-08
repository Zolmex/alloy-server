using System.Numerics;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using GameServer.Game.Worlds;
using GameServer.Utilities;

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
    
    public void MoveTowards(ref RealmTime time, ref WorldPosData moveTo, float tilesPerSecond) {
        var angle = this.GetAngleBetween(moveTo);
        var dist = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        var speed = Entity.GetSpeed(tilesPerSecond) * (time.ElapsedMsDelta / 1000f);
        dist *= speed;

        if (moveTo.DistSqr(Pos) < dist.LengthSquared()) {
            // If the distance we're about to move is greater than the distance to the desired position, set position to the desired position
            Move(moveTo.X, moveTo.Y);
            return;
        }

        Move(Pos + dist);
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
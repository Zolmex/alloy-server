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
    
    public void Init(WorldPosData spawn) {
        Pos = spawn;
        PrevPos = spawn;
        SpawnPos = spawn;
    }
    
    public void Move(in WorldPosData pos) {
        Move(pos.X, pos.Y);
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
    
    public void MoveTowards(ref RealmTime time, WorldPosData moveTo, float tilesPerSecond) {
        var angle = this.GetAngleBetween(moveTo);
        var dist = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        var speed = tilesPerSecond * (time.ElapsedMsDelta / 1000f);
        dist *= speed;

        if (moveTo.DistSqr(Pos) < dist.LengthSquared()) {
            // If the distance we're about to move is greater than the distance to the desired position, set position to the desired position
            Move(moveTo.X, moveTo.Y);
            return;
        }

        var newPos = (Vector2)Pos + dist;
        Move(newPos.X, newPos.Y);
    }
}
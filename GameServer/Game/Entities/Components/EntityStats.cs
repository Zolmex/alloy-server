using System;
using System.Buffers;
using System.Numerics;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Components;

public struct EntityStats : IEntityComponent {
    public const int STAT_COUNT = (int)StatType.StatTypeCount;

    public int Id { get; set; }

    public WorldPosData Pos;
    public MapTileData Tile;
    
    public readonly StatValue[] Stats;
    public BitMask256 StatUpdates;
    public BitMask256 PublicMask;
    public BitMask256 PrivateMask;

    private readonly World _world;
    private readonly EntityType _type;

    public EntityStats(World world, ref Entity en) {
        _world = world;
        _type = en.Type;
        
        Stats = ArrayPool<StatValue>.Shared.Rent(STAT_COUNT);
        Stats.AsSpan(0, STAT_COUNT).Clear();
        
        Set(StatType.Name, en.Desc.ObjectId);
    }

    public float GetSpeed(float speed) { // TODO: Condition effect system
        if (_type == EntityType.Player) {
            // if (p.HasConditionEffect(ConditionEffectIndex.Slowed))
            //     return 1;
            //
            // if (p.HasConditionEffect(ConditionEffectIndex.Speedy))
            //     speed *= 1.5f;

            var tileSpeedMult = Tile.Desc.Speed; // Sink level is not supported so just use the tile speed
            return speed * tileSpeedMult;
        }

        if (_type == EntityType.Character) {
            // if (chr.HasConditionEffect(ConditionEffectIndex.Slowed))
            //     return 1;
            //
            // if (chr.HasConditionEffect(ConditionEffectIndex.Speedy))
            //     speed *= 1.5f;
            return speed;
        }

        return speed;
    }
    
    public void MoveTowards(ref RealmTime time, ref Vector2 moveTo, float tilesPerSecond) {
        var angle = this.GetAngleBetween(moveTo);
        var dist = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        var speed = GetSpeed(tilesPerSecond) * (time.ElapsedMsDelta / 1000f);
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
        Set(StatType.PositionX, newX); // Internal, client doesn't care
        Set(StatType.PositionY, newY);
    }

    public int GetInt(StatType s) {
        return Stats[(int)s].IntVal;
    }

    public float GetFloat(StatType s) {
        return Stats[(int)s].FloatVal;
    }

    public string GetString(StatType s) {
        return Stats[(int)s].StrVal;
    }

    public void Set(StatType statType, int value, bool isPrivate = false) {
        SetInternal(statType, StatValue.FromInt(value), isPrivate);
    }

    public void Set(StatType statType, float value, bool isPrivate = false) {
        SetInternal(statType, StatValue.FromFloat(value), isPrivate);
    }

    public void Set(StatType statType, string value, bool isPrivate = false) {
        SetInternal(statType, StatValue.FromString(value), isPrivate);
    }
    
    private void SetInternal(StatType statType, StatValue sv, bool isPrivate) {
        var id = (int)statType;

        Stats[id] = sv;
        StatUpdates.Set(id);

        if (!isPrivate)
            PublicMask.Set(id);
        PrivateMask.Set(id);
    }

    public void Tick() {
        Tile = _world.Map.Tiles[(int)Pos.X, (int)Pos.Y];
        StatUpdates.Clear();
    }

    public void Dispose() {
        ArrayPool<StatValue>.Shared.Return(Stats);
    }
}
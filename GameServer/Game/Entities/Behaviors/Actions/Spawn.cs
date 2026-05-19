using System;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Common.Game;
using Common.Resources.Xml;
using Common.Utilities;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class SpawnInfo {
    public int CooldownMs;
    public int SpawnCount;
}

public record Spawn : BehaviorScript {
    private readonly int _cooldownMsDefault;
    private readonly int _cooldownOffsetMs;
    private readonly float _densityRadiusSqr;
    private readonly string _entityName;
    private readonly string[] _groupIds;
    private readonly int _maxDensity;
    private readonly int _maxSpawnCount;
    private readonly int _maxSpawnsPerReset;
    private readonly float _maxX;
    private readonly float _maxY;
    private readonly int _minSpawnCount;
    private readonly float _minX;
    private readonly float _minY;

    public Spawn(string entityName, float x = 0, float y = 0, int cooldownMs = 1000, int cooldownOffsetMs = 0,
        int maxSpawnsPerReset = int.MaxValue, int minSpawnCount = 1, int maxSpawnCount = 1, string group = null,
        int maxDensity = 0, float densityRadius = 10) {
        _entityName = entityName;
        _minX = x;
        _maxX = x;
        _minY = y;
        _maxY = y;
        _cooldownMsDefault = cooldownMs;
        _cooldownOffsetMs = cooldownOffsetMs;
        _maxSpawnsPerReset = maxSpawnsPerReset;
        _minSpawnCount = minSpawnCount;
        _maxSpawnCount = maxSpawnCount;
        if (_minSpawnCount > _maxSpawnCount)
            throw new ArithmeticException();
        _groupIds = group != null
            ? XmlLibrary.ObjectDescs.Values.Where(x => x.Group == group).Select(x => x.ObjectId).ToArray()
            : null;
        _maxDensity = maxDensity;
        _densityRadiusSqr = densityRadius * densityRadius;
    }

    public Spawn(string entityName, float minX, float maxX, float minY, float maxY, int cooldownMs = 1000,
        int cooldownOffsetMs = 0, int maxSpawnsPerReset = int.MaxValue,
        int minSpawnCount = 1, int maxSpawnCount = 1, string group = null, int maxDensity = 0,
        float densityRadius = 10) {
        _entityName = entityName;
        _minX = minX;
        _maxX = maxX;
        _minY = minY;
        _maxY = maxY;
        _cooldownMsDefault = cooldownMs;
        _cooldownOffsetMs = cooldownOffsetMs;
        _maxSpawnsPerReset = maxSpawnsPerReset;
        _minSpawnCount = minSpawnCount;
        _maxSpawnCount = maxSpawnCount;
        if (_minSpawnCount > _maxSpawnCount)
            throw new ArithmeticException();
        _groupIds = group != null
            ? XmlLibrary.ObjectDescs.Values.Where(x => x.Group == group).Select(x => x.ObjectId).ToArray()
            : null;
        _maxDensity = maxDensity;
        _densityRadiusSqr = densityRadius * densityRadius;
    }

    public override void Start(ref EntityView host) {
        var spawnInfo = host.Behavior.Resources.ResolveResource<SpawnInfo>(this);
        spawnInfo.CooldownMs = _cooldownOffsetMs;
        spawnInfo.SpawnCount = 0;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var spawnInfo = host.Behavior.Resources.ResolveResource<SpawnInfo>(this);
        if (spawnInfo.SpawnCount > _maxSpawnsPerReset) return BehaviorTickState.BehaviorFailed;
        if (spawnInfo.CooldownMs > 0) {
            spawnInfo.CooldownMs -= time.ElapsedMsDelta;
            if (spawnInfo.CooldownMs > 0)
                return BehaviorTickState.OnCooldown;
        }

        var random = new Random();
        var spawnCount = random.Next(_minSpawnCount, _maxSpawnCount);
        for (var i = 0; i < spawnCount; i++) {
            string enName;
            if (_groupIds == null)
                enName = _entityName;
            else
                enName = _groupIds.RandomElement();

            var nearbyCount = host.World.Map.GetEntitiesByName(host.Stats.Pos, enName, _densityRadiusSqr).Count();
            if (_maxDensity != 0 && nearbyCount >= _maxDensity)
                continue;

            var x = (float)random.NextDouble() * (_maxX - _minX) + _minX;
            var y = (float)random.NextDouble() * (_maxY - _minY) + _minY;
            var spawnX = host.Stats.Pos.X + x;
            var spawnY = host.Stats.Pos.Y + y;
            var objectType = XmlLibrary.Id2Object(enName).ObjectType;
            var isSpawned = host.Stats.Flags.IsSet((int)EntityFlags.Spawned);
            var world = host.World;
            var hostId = host.Id;

            GameLogic.Enqueue(() => {
                var child = new Entity(objectType);
                world.EnterWorld(ref child);
                ref var childBehavior = ref world.EntityBehaviors.Get(child.Id);
                childBehavior.ParentId = hostId;
                ref var childStats = ref world.EntityStats.Get(child.Id);
                childStats.Move(spawnX, spawnY);
                if (isSpawned)
                    childStats.Flags.Set((int)EntityFlags.Spawned);
            });

            spawnInfo.SpawnCount++;
            if (spawnInfo.SpawnCount == _maxSpawnCount)
                break;
        }

        spawnInfo.CooldownMs = _cooldownMsDefault;
        return BehaviorTickState.BehaviorActive;
    }
}
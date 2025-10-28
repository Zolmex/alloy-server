#region

using Common.Resources.Xml;
using Common.Utilities;
using System;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class SpawnInfo
    {
        public int CooldownMs;
        public int SpawnCount;
    }

    public record Spawn : BehaviorScript
    {
        private readonly string _entityName;
        private readonly float _minX;
        private readonly float _maxX;
        private readonly float _minY;
        private readonly float _maxY;
        private readonly int _cooldownMsDefault;
        private readonly int _cooldownOffsetMs;
        private readonly int _maxSpawnsPerReset;
        private readonly int _minSpawnCount;
        private readonly int _maxSpawnCount;
        private readonly string[] _groupIds;
        private readonly int _maxDensity;
        private readonly float _densityRadius;

        public Spawn(string entityName, float x = 0, float y = 0, int cooldownMs = 1000, int cooldownOffsetMs = 0, int maxSpawnsPerReset = int.MaxValue, int minSpawnCount = 1, int maxSpawnCount = 1, string group = null, int maxDensity = 0, float densityRadius = 10)
        {
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
            _groupIds = group != null ? XmlLibrary.ObjectDescs.Values.Where(x => x.Group == group).Select(x => x.ObjectId).ToArray() : null;
            _maxDensity = maxDensity;
            _densityRadius = densityRadius;
        }

        public Spawn(string entityName, float minX, float maxX, float minY, float maxY, int cooldownMs = 1000, int cooldownOffsetMs = 0, int maxSpawnsPerReset = int.MaxValue,
            int minSpawnCount = 1, int maxSpawnCount = 1, string group = null, int maxDensity = 0, float densityRadius = 10)
        {
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
            _groupIds = group != null ? XmlLibrary.ObjectDescs.Values.Where(x => x.Group == group).Select(x => x.ObjectId).ToArray() : null;
            _maxDensity = maxDensity;
            _densityRadius = densityRadius;
        }

        public Spawn(XElement xml)
        {
            _entityName = xml.GetAttribute<string>("entityName");
            _minX = xml.GetAttribute<float>("minX");
            _minY = xml.GetAttribute<float>("minY");
            _maxX = xml.GetAttribute<float>("maxX");
            _maxY = xml.GetAttribute<float>("maxY");
            _cooldownMsDefault = xml.GetAttribute<int>("cooldownMS", 1000);
            _cooldownOffsetMs = xml.GetAttribute<int>("cooldownOffsetMS", 0);
            _maxSpawnsPerReset = xml.GetAttribute<int>("maxSpawnsPerReset", int.MaxValue);
            _minSpawnCount = xml.GetAttribute<int>("minSpawnCount", 1);
            _maxSpawnCount = xml.GetAttribute<int>("maxSpawnCount", 1);
            if (_minSpawnCount > _maxSpawnCount)
                throw new ArithmeticException();
        }

        public override void Start(Character host)
        {
            var spawnInfo = host.ResolveResource<SpawnInfo>(this);
            spawnInfo.CooldownMs = _cooldownOffsetMs;
            spawnInfo.SpawnCount = 0;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var spawnInfo = host.ResolveResource<SpawnInfo>(this);
            if (spawnInfo.SpawnCount > _maxSpawnsPerReset) return BehaviorTickState.BehaviorFailed;
            if (spawnInfo.CooldownMs > 0)
            {
                spawnInfo.CooldownMs -= time.ElapsedMsDelta;
                if (spawnInfo.CooldownMs > 0)
                    return BehaviorTickState.OnCooldown;
            }

            var random = new Random();
            var spawnCount = random.Next(_minSpawnCount, _maxSpawnCount);
            for (var i = 0; i < spawnCount; i++)
            {
                string enName;
                if (_groupIds == null)
                    enName = _entityName;
                else
                    enName = _groupIds.RandomElement();

                if (_maxDensity != 0 && host.World.GetEnemiesByName(enName, host.Position.X, host.Position.Y, _densityRadius).Count() >= _maxDensity)
                    continue;

                var x = ((float)random.NextDouble() * (_maxX - _minX)) + _minX;
                var y = ((float)random.NextDouble() * (_maxY - _minY)) + _minY;
                var child = host.World.SpawnEntity(enName, new Vector2(host.Position.X + x, host.Position.Y + y));
                child.Parent = host;

                if (host.Spawned)
                { // Spawned by admin
                    child.Spawned = true;
                }

                spawnInfo.SpawnCount++;
                if (spawnInfo.SpawnCount == _maxSpawnCount)
                    break;
            }

            spawnInfo.CooldownMs = _cooldownMsDefault;
            return BehaviorTickState.BehaviorActive;
        }
    }
}
#region

using System.Linq;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class ReproduceInfo
    {
        public int CooldownMs;
    }

    public record Reproduce : BehaviorScript
    {
        private readonly string _entityName;
        private readonly int _cooldownMsDefault;
        private readonly int _maxDensity;
        private readonly float _densityRadius;

        public Reproduce(string entityName = null, int cooldownMs = 60000, int maxDensity = 0, float densityRadius = 10)
        {
            _entityName = entityName;
            _cooldownMsDefault = cooldownMs;
            _maxDensity = maxDensity;
            _densityRadius = densityRadius;
        }

        public override void Start(Character host)
        {
            var spawnInfo = host.ResolveResource<ReproduceInfo>(this);
            spawnInfo.CooldownMs = 0;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var spawnInfo = host.ResolveResource<ReproduceInfo>(this);
            if (spawnInfo.CooldownMs > 0)
            {
                spawnInfo.CooldownMs -= time.ElapsedMsDelta;
                if (spawnInfo.CooldownMs > 0)
                    return BehaviorTickState.OnCooldown;
            }

            var enName = _entityName ?? host.Desc.ObjectId;
            if (_maxDensity != 0 && host.World.GetEnemiesByName(enName, host.Position.X, host.Position.Y, _densityRadius).Count() >= _maxDensity)
                return BehaviorTickState.BehaviorFailed;

            var child = host.World.SpawnEntity(enName, new Vector2(host.Position.X, host.Position.Y));
            child.Parent = host;

            if (host.Spawned)
            { // Spawned by admin
                child.Spawned = true;
            }

            spawnInfo.CooldownMs = _cooldownMsDefault;
            return BehaviorTickState.BehaviorActive;
        }
    }
}
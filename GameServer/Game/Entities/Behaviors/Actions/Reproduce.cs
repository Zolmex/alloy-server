using System.Linq;
using System.Numerics;
using Common.Game;
using Common.Resources.Xml;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class ReproduceInfo {
    public int CooldownMs;
}

public record Reproduce : BehaviorScript {
    private readonly int _cooldownMsDefault;
    private readonly float _densityRadius;
    private readonly string _entityName;
    private readonly int _maxDensity;

    public Reproduce(string entityName = null, int cooldownMs = 60000, int maxDensity = 0, float densityRadius = 10) {
        _entityName = entityName;
        _cooldownMsDefault = cooldownMs;
        _maxDensity = maxDensity;
        _densityRadius = densityRadius;
    }

    public override void Start(ref EntityView host) {
        var spawnInfo = host.Behavior.Resources.ResolveResource<ReproduceInfo>(this);
        spawnInfo.CooldownMs = 0;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var spawnInfo = host.Behavior.Resources.ResolveResource<ReproduceInfo>(this);
        if (spawnInfo.CooldownMs > 0) {
            spawnInfo.CooldownMs -= time.ElapsedMsDelta;
            if (spawnInfo.CooldownMs > 0)
                return BehaviorTickState.OnCooldown;
        }

        var enName = _entityName ?? host.Entity.Desc.ObjectId;
        if (_maxDensity != 0 && host.World.Map.GetEntitiesByName(host.Stats.Pos, enName, _densityRadius).Count() >= _maxDensity)
            return BehaviorTickState.BehaviorFailed;

        var type = XmlLibrary.Id2Object(enName).ObjectType;
        var spawnX = host.Stats.Pos.X;
        var spawnY = host.Stats.Pos.Y;
        var isSpawned = host.Stats.Flags.IsSet((int)EntityFlags.Spawned);
        var world = host.World;
        var hostId = host.Id;

        host.World.Enqueue(_ => {
            var child = new Entity(type);
            world.EnterWorld(ref child);
            ref var childBehavior = ref world.EntityBehaviors.Get(child.Id);
            childBehavior.ParentId = hostId;
            ref var childStats = ref world.EntityStats.Get(child.Id);
            childStats.Move(spawnX, spawnY);
            if (isSpawned)
                childStats.Flags.Set((int)EntityFlags.Spawned);
        });

        spawnInfo.CooldownMs = _cooldownMsDefault;
        return BehaviorTickState.BehaviorActive;
    }
}
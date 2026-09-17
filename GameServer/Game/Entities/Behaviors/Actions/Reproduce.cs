using System.Linq;
using System.Numerics;
using Common.Game;
using Common.Resources.Xml;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Old;

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

    public override void Start(ref EntityContext host) {
        var spawnInfo = host.BehavController.Resources.ResolveResource<ReproduceInfo>(this);
        spawnInfo.CooldownMs = 0;
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var spawnInfo = host.BehavController.Resources.ResolveResource<ReproduceInfo>(this);
        if (spawnInfo.CooldownMs > 0) {
            spawnInfo.CooldownMs -= time.ElapsedMsDelta;
            if (spawnInfo.CooldownMs > 0)
                return BehaviorTickState.OnCooldown;
        }

        var enName = _entityName ?? host.Desc.ObjectId;
        if (_maxDensity != 0 && host.World.Map.GetEntitiesByName(host.Position.Pos, enName, _densityRadius).Count() >= _maxDensity)
            return BehaviorTickState.BehaviorFailed;

        var type = XmlLibrary.Id2Object(enName).ObjectType;
        var spawnX = host.Position.Pos.X;
        var spawnY = host.Position.Pos.Y;
        var isSpawned = host.Flags.Mask.IsSet((int)EntityFlags.Spawned);
        var world = host.World;
        var hostId = host.Entity;

        GameLogic.Enqueue(() => {
            var child = world.EnterWorld(XmlLibrary.ObjectDescs[type]);
            ref var childBehavior = ref world.Ecs.Get<Behavior>(child);
            childBehavior.Parent = hostId;
            ref var childPos = ref world.Ecs.Get<Position>(child);
            childPos.Move(spawnX, spawnY);
            if (isSpawned) {
                ref var childFlags = ref world.Ecs.Get<Flags>(child);
                childFlags.Mask.Set((int)EntityFlags.Spawned);
            }
        });

        spawnInfo.CooldownMs = _cooldownMsDefault;
        return BehaviorTickState.BehaviorActive;
    }
}
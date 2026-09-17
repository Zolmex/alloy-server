using System;
using System.Numerics;
using Arch.Core;
using Common;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class OrbitInfo {
    public int Direction;
    public float FinalRadius;
    public float FinalSpeed;
    public bool FirstTick;
    public Entity Target;
}

public record Orbit : BehaviorScript {
    private readonly float _acquireRange;
    private readonly bool _orbitClockwise;
    private readonly float _radius;
    private readonly float _radiusVariance;
    private readonly float _speed;
    private readonly float _speedVariance;
    private readonly string _target;
    private readonly bool _targetPlayer;

    public Orbit(float speed, float radius, float acquireRange = 10, string target = null,
        float speedVariance = 0.0f, float radiusVariance = 0.0f, bool orbitClockwise = false,
        bool targetPlayer = false) {
        _speed = speed;
        _radius = radius;
        _radiusVariance = radiusVariance;
        _acquireRange = acquireRange;
        _speedVariance = speedVariance;
        _orbitClockwise = orbitClockwise;
        _target = target;
        _targetPlayer = targetPlayer;
    }

    public override void Start(ref EntityContext host) {
        var orbitInfo = host.BehavController.Resources.ResolveResource<OrbitInfo>(this);
        orbitInfo.Direction = _orbitClockwise ? 1 : -1;
        orbitInfo.FinalSpeed = _speed + _speedVariance * (float)(Random.Shared.NextDouble() * 2 - 1);
        orbitInfo.FinalRadius = _radius + _radiusVariance * (float)(Random.Shared.NextDouble() * 2 - 1);
        orbitInfo.FirstTick = true;
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var orbitInfo = host.BehavController.Resources.ResolveResource<OrbitInfo>(this);
        // if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed)) // TODO: condition effects
        //     return BehaviorTickState.BehaviorFailed;

        Entity target;
        if (_targetPlayer)
            target = host.World.Map.GetNearestPlayer(host.Position.Pos, _acquireRange * _acquireRange);
        else
            target = orbitInfo.Target == Entity.Null ? host.World.Map.GetNearestOtherEntityByName(host.Position.Pos, host.Entity, _target, _acquireRange) : orbitInfo.Target;

        orbitInfo.Target = target;

        
        if (target == Entity.Null) {
            return BehaviorTickState.BehaviorFailed;
        }

        ref var targetPos = ref host.World.Ecs.Get<Position>(target);
        var angle = host.Position.Pos.Y == targetPos.Pos.Y && host.Position.Pos.X == targetPos.Pos.X
            ? Math.Atan2(host.Position.Pos.Y - targetPos.Pos.Y + (Random.Shared.NextDouble() * 2 - 1),
                host.Position.Pos.X - targetPos.Pos.X + (Random.Shared.NextDouble() * 2 - 1))
            : Math.Atan2(host.Position.Pos.Y - targetPos.Pos.Y, host.Position.Pos.X - targetPos.Pos.X);
        var angularSpd = orbitInfo.Direction * host.GetSpeed(orbitInfo.FinalSpeed) / orbitInfo.FinalRadius;

        angle += angularSpd * (time.ElapsedMsDelta / 1000f);

        var x = targetPos.Pos.X + Math.Cos(angle) * orbitInfo.FinalRadius;
        var y = targetPos.Pos.Y + Math.Sin(angle) * orbitInfo.FinalRadius;
        var vect = new Vector2((float)x, (float)y) - new Vector2(host.Position.Pos.X, host.Position.Pos.Y);
        vect = Vector2.Normalize(vect);
        vect *= host.GetSpeed(orbitInfo.FinalSpeed) * (time.ElapsedMsDelta / 1000f);

        var newX = host.Position.Pos.X + vect.X;
        var newY = host.Position.Pos.Y + vect.Y;
        host.Position.Move(newX, newY);

        if (orbitInfo.FirstTick) {
            orbitInfo.FirstTick = false;
            return BehaviorTickState.BehaviorActivate;
        }

        return BehaviorTickState.BehaviorActive;
    }
}
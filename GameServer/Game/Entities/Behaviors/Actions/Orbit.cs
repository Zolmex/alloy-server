using System;
using System.Numerics;
using Common;
using Common.Game;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class OrbitInfo {
    public int Direction;
    public float FinalRadius;
    public float FinalSpeed;
    public bool FirstTick;
    public int TargetId;
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

    public override void Start(ref EntityView host) {
        var orbitInfo = host.Behavior.Resources.ResolveResource<OrbitInfo>(this);
        orbitInfo.Direction = _orbitClockwise ? 1 : -1;
        orbitInfo.FinalSpeed = _speed + _speedVariance * (float)(Random.Shared.NextDouble() * 2 - 1);
        orbitInfo.FinalRadius = _radius + _radiusVariance * (float)(Random.Shared.NextDouble() * 2 - 1);
        orbitInfo.FirstTick = true;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var orbitInfo = host.Behavior.Resources.ResolveResource<OrbitInfo>(this);
        // if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed)) // TODO: condition effects
        //     return BehaviorTickState.BehaviorFailed;

        int targetId;
        if (_targetPlayer)
            targetId = host.World.Map.GetNearestPlayer(host.Stats.Pos, _acquireRange * _acquireRange);
        else
            targetId = orbitInfo.TargetId == 0 ? host.World.Map.GetNearestOtherEntityByName(host.Stats.Pos, host.Id, _target, _acquireRange) : orbitInfo.TargetId;

        orbitInfo.TargetId = targetId;

        
        if (targetId == 0) {
            return BehaviorTickState.BehaviorFailed;
        }

        ref var target = ref host.World.EntityStats.Get(targetId);
        var angle = host.Stats.Pos.Y == target.Pos.Y && host.Stats.Pos.X == target.Pos.X
            ? Math.Atan2(host.Stats.Pos.Y - target.Pos.Y + (Random.Shared.NextDouble() * 2 - 1),
                host.Stats.Pos.X - target.Pos.X + (Random.Shared.NextDouble() * 2 - 1))
            : Math.Atan2(host.Stats.Pos.Y - target.Pos.Y, host.Stats.Pos.X - target.Pos.X);
        var angularSpd = orbitInfo.Direction * host.Stats.GetSpeed(orbitInfo.FinalSpeed) / orbitInfo.FinalRadius;

        angle += angularSpd * (time.ElapsedMsDelta / 1000f);

        var x = target.Pos.X + Math.Cos(angle) * orbitInfo.FinalRadius;
        var y = target.Pos.Y + Math.Sin(angle) * orbitInfo.FinalRadius;
        var vect = new Vector2((float)x, (float)y) - new Vector2(host.Stats.Pos.X, host.Stats.Pos.Y);
        vect = Vector2.Normalize(vect);
        vect *= host.Stats.GetSpeed(orbitInfo.FinalSpeed) * (time.ElapsedMsDelta / 1000f);

        var newX = host.Stats.Pos.X + vect.X;
        var newY = host.Stats.Pos.Y + vect.Y;
        host.Stats.Move(newX, newY);

        if (orbitInfo.FirstTick) {
            orbitInfo.FirstTick = false;
            return BehaviorTickState.BehaviorActivate;
        }

        return BehaviorTickState.BehaviorActive;
    }
}
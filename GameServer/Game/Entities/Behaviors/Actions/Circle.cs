using System;
using System.Numerics;
using System.Xml.Linq;
using Arch.Core;
using Common.Game;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Behaviors;

public class CircleInfo {
    public float CurrentAngle;
}

public record Circle : BehaviorScript {
    private readonly float _acquireRadiusSqr;
    private readonly float _radius;
    private readonly float _rotationsPerSecond;
    private readonly string _target;

    public Circle(float rotationsPerSecond = 1f, float acquireRadius = 20f, float radius = 4f,
        string target = "player") {
        _rotationsPerSecond = rotationsPerSecond;
        _acquireRadiusSqr = (float)Math.Pow(acquireRadius, 2);
        _radius = radius;
        _target = target;
    }

    public override void Start(ref EntityContext host) {
        ref var hostPos = ref host.Position;
        
        Entity target;
        if (_target == "player")
            target = host.World.Map.GetNearestPlayer(hostPos.Pos, _acquireRadiusSqr);
        else
            target = host.World.Map.GetNearestEntityByName(_target, hostPos.Pos.X, hostPos.Pos.Y, _acquireRadiusSqr);

        if (target == Entity.Null)
            return;

        ref var targetPos = ref host.World.Ecs.Get<Position>(target);
        var resource = host.Behavior.Resources.ResolveResource<CircleInfo>(this);
        resource.CurrentAngle = hostPos.GetAngleBetween(targetPos.Pos).Rad2Deg();
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var resource = host.Behavior.Resources.ResolveResource<CircleInfo>(this);
        var angleInc = 360f * (_rotationsPerSecond * time.ElapsedMsDelta / 1000);

        ref var hostPos = ref host.Position;
        
        Entity target;
        if (_target == "player")
            target = host.World.Map.GetNearestPlayer(hostPos.Pos, _acquireRadiusSqr);
        else
            target = host.World.Map.GetNearestEntityByName(_target, hostPos.Pos.X, hostPos.Pos.Y, _acquireRadiusSqr);

        if (target == Entity.Null)
            return BehaviorTickState.BehaviorFailed;

        ref var targetPos = ref host.World.Ecs.Get<Position>(target);
        hostPos.Move(
            targetPos.Pos.X + MathF.Cos(resource.CurrentAngle.Deg2Rad()) * _radius,
            targetPos.Pos.Y + MathF.Sin(resource.CurrentAngle.Deg2Rad()) * _radius);
        resource.CurrentAngle += angleInc;
        return BehaviorTickState.BehaviorActive;
    }
}
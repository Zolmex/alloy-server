using System;
using System.Numerics;
using System.Xml.Linq;
using Common.Game;
using Common.Utilities;
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

    public Circle(XElement xml) {
        _rotationsPerSecond = xml.GetAttribute("rotationsPerSecond", 1f);
        _acquireRadiusSqr = (float)Math.Pow(xml.GetAttribute("acquireRadius", 20f), 2);
        _radius = xml.GetAttribute("radius", 1f);
        _target = xml.GetAttribute("target", "player");
    }

    public Circle(float rotationsPerSecond = 1f, float acquireRadius = 20f, float radius = 4f,
        string target = "player") {
        _rotationsPerSecond = rotationsPerSecond;
        _acquireRadiusSqr = (float)Math.Pow(acquireRadius, 2);
        _radius = radius;
        _target = target;
    }

    public override void Start(ref EntityView host) {
        int targetId;
        if (_target == "player")
            targetId = host.World.Map.GetNearestPlayer(host.Stats.Pos, _acquireRadiusSqr);
        else
            targetId = host.World.Map.GetNearestEntityByName(_target, host.Stats.Pos.X, host.Stats.Pos.Y, _acquireRadiusSqr);

        if (targetId == 0)
            return;

        ref var target = ref host.World.EntityStats.Get(targetId);
        var resource = host.Behavior.Resources.ResolveResource<CircleInfo>(this);
        resource.CurrentAngle = host.Stats.GetAngleBetween(ref target).Rad2Deg();
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var resource = host.Behavior.Resources.ResolveResource<CircleInfo>(this);
        var angleInc = 360f * (_rotationsPerSecond * time.ElapsedMsDelta / 1000);

        int targetId;
        if (_target == "player")
            targetId = host.World.Map.GetNearestPlayer(host.Stats.Pos, _acquireRadiusSqr);
        else
            targetId = host.World.Map.GetNearestEntityByName(_target, host.Stats.Pos.X, host.Stats.Pos.Y, _acquireRadiusSqr);

        if (targetId == 0)
            return BehaviorTickState.BehaviorFailed;

        ref var target = ref host.World.EntityStats.Get(targetId);
        var targetPos = new Vector2(target.Pos.X, target.Pos.Y);
        targetPos +=
            new Vector2(MathF.Cos(resource.CurrentAngle.Deg2Rad()), MathF.Sin(resource.CurrentAngle.Deg2Rad())) *
            _radius;
        host.Stats.Move(targetPos);
        resource.CurrentAngle += angleInc;
        return BehaviorTickState.BehaviorActive;
    }
}
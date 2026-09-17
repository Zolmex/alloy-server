using System;
using System.Numerics;
using Arch.Core;
using Common;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class SwirlInfo {
    public bool Acquired;
    public Vector2 Center;
    public int RemainingTime;
}

public record Swirl : BehaviorScript {
    private readonly float _acquireRange;
    private readonly float _radius;
    private readonly float _speed;
    private readonly bool _targeted;

    public Swirl(float speed = 1, float radius = 8, float acquireRange = 10, bool targeted = true) {
        _speed = speed;
        _radius = radius;
        _acquireRange = acquireRange;
        _targeted = targeted;
    }

    public override void Start(ref EntityContext host) {
        var swirlState = host.Behavior.Resources.ResolveResource<SwirlInfo>(this);
        swirlState.Center = _targeted ? Vector2.Zero : new Vector2(host.Position.Pos.X, host.Position.Pos.Y);
        swirlState.Acquired = !_targeted;
        swirlState.RemainingTime = 0;
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var swirlState = host.Behavior.Resources.ResolveResource<SwirlInfo>(this);
        // if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed)) // TODO: condition effects
        //     return BehaviorTickState.BehaviorFailed;

        var period = (int)(1000 * _radius / host.GetSpeed(_speed) * (2 * Math.PI));

        if (!swirlState.Acquired && swirlState.RemainingTime <= 0 && _targeted) {
            var entityId = host.World.Map.GetNearestOtherEntityByName(host.Position.Pos, host.Entity, null, _acquireRange);
            ref var enPos = ref host.World.Ecs.Get<Position>(entityId);
            if (entityId != Entity.Null && enPos.Pos.X != host.Position.Pos.X && enPos.Pos.Y != host.Position.Pos.Y) {
                //find circle which pass through host and player pos
                var l = enPos.DistSqr(ref host.Position);
                var hx = (host.Position.Pos.X + enPos.Pos.X) / 2;
                var hy = (host.Position.Pos.Y + enPos.Pos.Y) / 2;
                var c = Math.Sqrt(Math.Abs(_radius * _radius - l) / 4);

                swirlState.Center = new Vector2((float)(hx + c * (host.Position.Pos.Y - enPos.Pos.Y) / l),
                    (float)(hy + c * (enPos.Pos.X - host.Position.Pos.X) / (l / l)));
                swirlState.RemainingTime = period;
                swirlState.Acquired = true;
            }
            else {
                swirlState.Acquired = false;
            }
        }
        else if (swirlState.RemainingTime <= 0 ||
                 (swirlState.RemainingTime - period > 200 && host.World.Map.GetNearestOtherEntityByName(host.Position.Pos, host.Entity, null, 2) != null)) {
            if (_targeted) {
                swirlState.Acquired = false;

                var entityId = host.World.Map.GetNearestOtherEntityByName(host.Position.Pos, host.Entity, null, _acquireRange);

                if (entityId != Entity.Null)
                    swirlState.RemainingTime = 0;
                else
                    swirlState.RemainingTime = 5000;
            }
            else {
                swirlState.RemainingTime = 5000;
            }
        }
        else {
            swirlState.RemainingTime -= time.ElapsedMsDelta;
        }

        var angle = host.Position.Pos.Y == swirlState.Center.Y && host.Position.Pos.X == swirlState.Center.X
            ? Math.Atan2(host.Position.Pos.Y - swirlState.Center.Y + (Random.Shared.NextDouble() * 2 - 1),
                host.Position.Pos.X - swirlState.Center.X + (Random.Shared.NextDouble() * 2 - 1))
            : Math.Atan2(host.Position.Pos.Y - swirlState.Center.Y, host.Position.Pos.X - swirlState.Center.X);
        var spd = host.GetSpeed(_speed) * (swirlState.Acquired ? 1 : 0.2);
        var angularSpd = spd / _radius;

        angle += angularSpd * time.ElapsedMsDelta;

        var x = swirlState.Center.X + Math.Cos(angle) * _radius;
        var y = swirlState.Center.Y + Math.Sin(angle) * _radius;
        var vect = new Vector2((float)x, (float)y) - new Vector2(host.Position.Pos.X, host.Position.Pos.Y);
        vect = Vector2.Normalize(vect);
        vect *= (float)spd * time.ElapsedMsDelta;

        var newX = vect.X + host.Position.Pos.X;
        var newY = vect.Y + host.Position.Pos.Y;
        host.Position.Move(newX, newY);

        return BehaviorTickState.BehaviorActive;
    }
}
using System;
using System.Numerics;
using Common;
using Common.Game;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class MoveLineInfo {
    public float DistLeft;
}

public record MoveLine : BehaviorScript {
    private readonly float _angle;
    private readonly float _distance;
    private readonly float _speed;

    public MoveLine(float speed, float angle = 0, float distance = 0) {
        _speed = speed;
        _angle = angle.Deg2Rad();
        _distance = distance;
    }

    public override void Start(ref EntityView host) {
        var state = host.Behavior.Resources.ResolveResource<MoveLineInfo>(this);
        state.DistLeft = _distance;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        ref var stats = ref host.World.EntityStats.Get(host.Id);
        var state = host.Behavior.Resources.ResolveResource<MoveLineInfo>(this);
        // if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed)) // TODO: Condition effects
        //     return BehaviorTickState.BehaviorFailed;

        var vect = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle));
        vect += stats.Pos.ToVec2();
        if (state.DistLeft > 0) {
            var moveDist = stats.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
            state.DistLeft -= moveDist;
        }

        stats.MoveTowards(ref time, ref vect, _speed);

        return BehaviorTickState.BehaviorActive;
    }
}
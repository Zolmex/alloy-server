#region

using System;
using System.Numerics;
using Common;
using Common.Structs;
using Common.Utilities;
using GameServerOld.Game.Entities.Types;

#endregion

namespace GameServerOld.Game.Entities.Behaviors.Actions;

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

    public override void Start(CharacterEntity host) {
        var state = host.ResolveResource<MoveLineInfo>(this);
        state.DistLeft = _distance;
    }

    public override BehaviorTickState Tick(CharacterEntity host, RealmTime time) {
        var state = host.ResolveResource<MoveLineInfo>(this);
        if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
            return BehaviorTickState.BehaviorFailed;

        var vect = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle));
        vect += host.Position.ToVec2();
        if (state.DistLeft > 0) {
            var moveDist = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
            state.DistLeft -= moveDist;
        }

        host.MoveTowards(time, vect, _speed);

        return BehaviorTickState.BehaviorActive;
    }
}
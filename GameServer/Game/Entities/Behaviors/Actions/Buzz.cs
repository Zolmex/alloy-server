#region

using System;
using System.Numerics;
using Common;
using GameServer.Game.Entities.Types;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions;

public class BuzzInfo {
    public Vector2 Direction;
    public float RemainingDistance;
}

public record Buzz : BehaviorScript {
    private readonly float _distance;
    private readonly float _speed;

    public Buzz(float speed = 2, float distance = 0.5f) {
        _speed = speed;
        _distance = distance;
    }

    public override void Start(CharacterEntity host) {
        var buzzState = host.ResolveResource<BuzzInfo>(this);
    }

    public override BehaviorTickState Tick(CharacterEntity host, RealmTime time) {
        var buzzState = host.ResolveResource<BuzzInfo>(this);
        if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
            return BehaviorTickState.BehaviorFailed;

        if (buzzState.RemainingDistance <= 0) {
            do {
                buzzState.Direction = new Vector2(Random.Shared.Next(-1, 2), Random.Shared.Next(-1, 2));
            } while (buzzState.Direction.X == 0 && buzzState.Direction.Y == 0);

            buzzState.Direction = Vector2.Normalize(buzzState.Direction);
            buzzState.RemainingDistance = _distance;
        }

        var dist = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);

        host.MoveRelative(buzzState.Direction.X * dist, buzzState.Direction.Y * dist);

        buzzState.RemainingDistance -= dist;

        return BehaviorTickState.BehaviorActive;
    }
}
using System;
using System.Numerics;
using Common;
using Common.Game;

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

    public override void Start(ref EntityView host) {
        var buzzState = host.Behavior.Resources.ResolveResource<BuzzInfo>(this);
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var buzzState = host.Behavior.Resources.ResolveResource<BuzzInfo>(this);
        // if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed)) // TODO: condition effects
        //     return BehaviorTickState.BehaviorFailed;

        if (buzzState.RemainingDistance <= 0) {
            do {
                buzzState.Direction = new Vector2(Random.Shared.Next(-1, 2), Random.Shared.Next(-1, 2));
            } while (buzzState.Direction.X == 0 && buzzState.Direction.Y == 0);

            buzzState.Direction = Vector2.Normalize(buzzState.Direction);
            buzzState.RemainingDistance = _distance;
        }

        var dist = host.Stats.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);

        var newX = host.Stats.Pos.X + buzzState.Direction.X * dist;
        var newY = host.Stats.Pos.Y + buzzState.Direction.Y * dist;
        host.Stats.Move(newX, newY);

        buzzState.RemainingDistance -= dist;

        return BehaviorTickState.BehaviorActive;
    }
}
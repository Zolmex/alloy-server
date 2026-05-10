using System.Numerics;
using Common;
using Common.Game;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class ChargeInfo {
    public Vector2 Direction;
    public int RemainingTime;
}

public record Charge : BehaviorScript {
    private readonly int _cooldownMS;
    private readonly float _range;
    private readonly float _speed;

    public Charge(float speed = 1, float range = 10, int cooldownMS = 1000) {
        _speed = speed;
        _range = range;
        _cooldownMS = cooldownMS;
    }

    public override void Start(ref EntityView host) {
        var chargeState = host.Behavior.Resources.ResolveResource<ChargeInfo>(this);
        chargeState.RemainingTime = 0; // Make sure the behavior runs once
        chargeState.Direction = Vector2.Zero;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var chargeState = host.Behavior.Resources.ResolveResource<ChargeInfo>(this);
        // if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed)) // TODO: condition effects
        //     return BehaviorTickState.BehaviorFailed;

        var status = BehaviorTickState.BehaviorActive;
        if (chargeState.RemainingTime <= 0) {
            if (chargeState.Direction == Vector2.Zero) {
                var plrId = host.World.Map.GetNearestPlayer(host.Stats.Pos, _range);
                if (plrId == 0)
                    return status;
                
                ref var player = ref host.World.EntityStats.Get(plrId);
                if (player.Pos.X != host.Stats.Pos.X && player.Pos.Y != host.Stats.Pos.Y) {
                    chargeState.Direction = new Vector2(player.Pos.X - host.Stats.Pos.X,
                        player.Pos.Y - host.Stats.Pos.Y);

                    var d = chargeState.Direction.Length();

                    chargeState.Direction = Vector2.Normalize(chargeState.Direction);
                    chargeState.RemainingTime = (int)(d / host.Stats.GetSpeed(_speed) * 1000);

                    status = BehaviorTickState.BehaviorActivate;
                }
            }
            else {
                chargeState.Direction = Vector2.Zero;
                chargeState.RemainingTime = _cooldownMS;

                status = BehaviorTickState.BehaviorDeactivate;
            }
        }

        if (chargeState.Direction != Vector2.Zero) {
            var dist = host.Stats.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
            var newX = host.Stats.Pos.X + chargeState.Direction.X * dist;
            var newY = host.Stats.Pos.Y + chargeState.Direction.Y * dist;
            host.Stats.Move(newX, newY);
        }

        chargeState.RemainingTime -= time.ElapsedMsDelta;
        return status;
    }
}
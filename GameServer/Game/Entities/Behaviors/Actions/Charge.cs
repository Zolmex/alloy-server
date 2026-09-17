using System.Numerics;
using Arch.Core;
using Common;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;

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

    public override void Start(ref EntityContext host) {
        var chargeState = host.BehavController.Resources.ResolveResource<ChargeInfo>(this);
        chargeState.RemainingTime = 0; // Make sure the behavior runs once
        chargeState.Direction = Vector2.Zero;
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var chargeState = host.BehavController.Resources.ResolveResource<ChargeInfo>(this);
        // if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed)) // TODO: condition effects
        //     return BehaviorTickState.BehaviorFailed;

        ref var hostPos = ref host.Position;
        var status = BehaviorTickState.BehaviorActive;
        if (chargeState.RemainingTime <= 0) {
            if (chargeState.Direction == Vector2.Zero) {
                var player = host.World.Map.GetNearestPlayer(hostPos.Pos, _range);
                if (player == Entity.Null)
                    return status;

                ref var plrPos = ref host.World.Ecs.Get<Position>(player);
                if (plrPos.Pos.X != hostPos.Pos.X && plrPos.Pos.Y != hostPos.Pos.Y) {
                    chargeState.Direction = new Vector2(plrPos.Pos.X - hostPos.Pos.X,
                        plrPos.Pos.Y - hostPos.Pos.Y);

                    var d = chargeState.Direction.Length();

                    chargeState.Direction = Vector2.Normalize(chargeState.Direction);
                    chargeState.RemainingTime = (int)(d / host.GetSpeed(_speed) * 1000);

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
            var dist = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
            var newX = hostPos.Pos.X + chargeState.Direction.X * dist;
            var newY = hostPos.Pos.Y + chargeState.Direction.Y * dist;
            hostPos.Move(newX, newY);
        }

        chargeState.RemainingTime -= time.ElapsedMsDelta;
        return status;
    }
}
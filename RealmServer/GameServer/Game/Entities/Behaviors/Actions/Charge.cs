#region

using Common;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class ChargeInfo
    {
        public Vector2 Direction;
        public int RemainingTime;
    }

    public record Charge : BehaviorScript
    {
        private readonly float _range;
        private readonly float _speed;
        private readonly int _cooldownMS;

        public Charge(float speed = 1, float range = 10, int cooldownMS = 1000)
        {
            _speed = speed;
            _range = range;
            _cooldownMS = cooldownMS;
        }

        public override void Start(Character host)
        {
            var chargeState = host.ResolveResource<ChargeInfo>(this);
            chargeState.RemainingTime = 0; // Make sure the behavior runs once
            chargeState.Direction = Vector2.Zero;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var chargeState = host.ResolveResource<ChargeInfo>(this);
            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return BehaviorTickState.BehaviorFailed;

            var status = BehaviorTickState.BehaviorActive;
            if (chargeState.RemainingTime <= 0)
            {
                if (chargeState.Direction == Vector2.Zero)
                {
                    var player = host.GetNearestPlayer(_range);
                    if (player != null && player.Position.X != host.Position.X && player.Position.Y != host.Position.Y)
                    {
                        chargeState.Direction = new Vector2(player.Position.X - host.Position.X, player.Position.Y - host.Position.Y);

                        var d = chargeState.Direction.Length();

                        chargeState.Direction = Vector2.Normalize(chargeState.Direction);
                        chargeState.RemainingTime = (int)(d / host.GetSpeed(_speed) * 1000);

                        status = BehaviorTickState.BehaviorActivate;
                    }
                }
                else
                {
                    chargeState.Direction = Vector2.Zero;
                    chargeState.RemainingTime = _cooldownMS;

                    status = BehaviorTickState.BehaviorDeactivate;
                }
            }

            if (chargeState.Direction != Vector2.Zero)
            {
                var dist = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
                host.MoveRelative(chargeState.Direction.X * dist, chargeState.Direction.Y * dist);
            }

            chargeState.RemainingTime -= time.ElapsedMsDelta;
            return status;
        }
    }
}
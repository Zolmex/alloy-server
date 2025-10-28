#region

using Common;
using System;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public enum ProtectState
    {
        DontKnowWhere,
        Protecting,
        Protected
    }

    public class ProtectInfo
    {
        public ProtectState State;
    }

    public record Protect : BehaviorScript
    {
        private readonly float _acquireRange;
        private readonly string _protectee;
        private readonly float _protectionRange;
        private readonly float _reprotectRange;
        private readonly float _speed;

        public Protect(float speed, string protectee, float acquireRange = 10, float protectionRange = 2, float reprotectRange = 1)
        {
            _acquireRange = acquireRange;
            _protectee = protectee;
            _protectionRange = protectionRange;
            _reprotectRange = reprotectRange;
            _speed = speed;
        }

        public override void Start(Character host)
        {
            var protectInfo = host.ResolveResource<ProtectInfo>(this);
            protectInfo.State = ProtectState.DontKnowWhere;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var protectInfo = host.ResolveResource<ProtectInfo>(this);
            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
                return BehaviorTickState.BehaviorFailed;

            Vector2 vect;
            var s = protectInfo.State;
            var entity = host.GetNearestOtherEnemyByName(_protectee, _acquireRange);
            switch (s)
            {
                case ProtectState.DontKnowWhere:
                    if (entity != null)
                    {
                        s = ProtectState.Protecting;

                        goto case ProtectState.Protecting;
                    }

                    break;

                case ProtectState.Protecting:
                    if (entity == null)
                    {
                        s = ProtectState.DontKnowWhere;

                        break;
                    }

                    vect = new Vector2(entity.Position.X - host.Position.X, entity.Position.Y - host.Position.Y);
                    if (vect.Length() > _reprotectRange)
                    {
                        vect = Vector2.Normalize(vect);

                        var dist = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
                        host.MoveRelative(vect.X * dist, vect.Y * dist);
                    }
                    else
                    {
                        s = ProtectState.Protected;
                    }

                    break;

                case ProtectState.Protected:
                    if (entity == null)
                    {
                        s = ProtectState.DontKnowWhere;

                        break;
                    }

                    vect = new Vector2(entity.Position.X - host.Position.X, entity.Position.Y - host.Position.Y);
                    if (vect.Length() > _protectionRange)
                    {
                        s = ProtectState.Protecting;

                        goto case ProtectState.Protecting;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            protectInfo.State = s;
            return BehaviorTickState.BehaviorActive;
        }
    }
}
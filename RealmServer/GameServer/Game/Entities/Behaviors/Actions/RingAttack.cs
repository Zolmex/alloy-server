#region

using Common;
using Common.ProjectilePaths;
using Common.Utilities;
using System;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class RingAttackInfo
    {
        public float AngleToIncrement;
        public int CoolDownLeft;
        public float FixedAngle;
        public float SavedAngle;
        public bool Targeted;
    }

    public record RingAttack : BehaviorScript
    {
        private readonly int _count;
        private readonly float _radius;
        private readonly float _angleOffset;
        private readonly int _projectileIndex;
        private readonly float _angleToIncrement;
        private readonly float _fixedAngle;
        private readonly bool _targeted;
        private readonly int _coolDownMS;
        private readonly bool _seeInvis;
        private readonly bool _useSavedAngle;

        public RingAttack(float radius, int count, float offset, int projectileIndex, float angleToIncrement,
            float fixedAngle = 0, bool targeted = false, int coolDownMS = 0, bool seeInvis = false,
            bool useSavedAngle = false)
        {
            _count = count;
            _radius = radius;
            _angleOffset = offset;
            _projectileIndex = projectileIndex;
            _angleToIncrement = angleToIncrement.Deg2Rad();
            _fixedAngle = fixedAngle.Deg2Rad();
            _coolDownMS = coolDownMS;
            _seeInvis = seeInvis;
            _useSavedAngle = useSavedAngle;
        }

        public override void Start(Character host)
        {
            var state = host.ResolveResource<RingAttackInfo>(this);
            state.AngleToIncrement = _angleToIncrement;
            state.FixedAngle = _fixedAngle;
            state.CoolDownLeft = _coolDownMS;
            state.Targeted = _targeted;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var state = host.ResolveResource<RingAttackInfo>(this);

            if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
                return BehaviorTickState.BehaviorFailed;

            if (state.CoolDownLeft > 0)
            {
                state.CoolDownLeft -= time.ElapsedMsDelta;
                return BehaviorTickState.OnCooldown;
            }

            var entity = _radius == 0 ? null : host.GetNearestOtherEnemyByName(null, _radius);
            var angleInc = 2 * MathF.PI / _count;
            var desc = host.Desc.Projectiles[(byte)_projectileIndex];

            float angle = 0;
            if (state.Targeted)
            {
                if (state.AngleToIncrement != 0)
                {
                    if (_useSavedAngle)
                        state.FixedAngle = state.SavedAngle;

                    state.FixedAngle += state.AngleToIncrement;
                    state.SavedAngle = state.FixedAngle;
                }

                angle = state.FixedAngle;
            }
            else
                angle = entity == null ? _angleOffset : (float)Math.Atan2(entity.Position.Y - host.Position.Y, entity.Position.X - host.Position.X) + _angleOffset;

            var count = _count;
            if (host.HasConditionEffect(ConditionEffectIndex.Dazed))
                count = Math.Max(1, count / 2);

            var dmg = Random.Shared.Next(desc.MinDamage, desc.MaxDamage);
            var startAngle = angle * (count - 1) / 2;

            host.ShootProjectiles(ProjectilePathSegment.ParsePath(desc).ToPath(), desc.ObjectType, dmg, dmg, (byte)count, startAngle.Rad2Deg(),
                host.Position.X, host.Position.Y, angleInc.Rad2Deg(), desc.MultiHit,
                desc.PassesCover, desc.ArmorPiercing, null);

            state.CoolDownLeft = time.ElapsedMsDelta;
            return BehaviorTickState.BehaviorActive;
        }
    }
}
#region

using Common;
using Common.Utilities;
using GameServer.Utilities;
using System;
using System.Numerics;
using System.Xml.Linq;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    /// <summary>
    /// A behavior for creating short burst dashes towards a given target.
    /// A collection of dashes is referred to as a 'Cycle', for when you need multiple dashes to be chained.
    /// </summary>
    public record Dash : BehaviorScript
    {
        private readonly float acquireRadiusSqr;
        private readonly int numDashes;
        private readonly float dashTime;
        private readonly int dashTimeMs;
        private readonly float dashRange;
        private readonly int cooldownMS;
        private readonly int cooldownOffsetMS;
        private readonly int cycleCooldownMS;
        private readonly float angleOffset;
        private readonly int damage;
        private readonly Ease ease;
        private readonly TargetType targetType;
        private readonly float fixedAngle;
        private readonly float dashDamageRadius;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dash"/> class.
        /// </summary>
        /// <param name="radius">Within how many tiles does a target need to be to have this behavior run.</param>
        /// <param name="numDashes">Number of dashes that should be ran before a cycle ends.</param>
        /// <param name="dashSpeed">How tiles per second the dash should travel.</param>
        /// <param name="dashRange">How many tiles the dash should travel.</param>
        /// <param name="cooldownMs">How many milliseconds should the cooldown be between dashes in a cycle.</param>
        /// <param name="cooldownOffsetMs">How many milliseconds should the behavior start after.</param>
        /// <param name="cycleCooldownMs">How many milliseconds should the behavior wait between cycles.</param>
        /// <param name="angleOffset">How many degrees should the dash angle be offset by.</param>
        /// <param name="damage">How much damage should the dash do when it passes through its target (players only).</param>
        /// <param name="ease">Which Easing type should be used for the dash movement. This is used for non linear speed scaling to make movements look more natural.</param>
        /// <param name="targetType">How should the behavior pick its target.</param>
        /// <param name="fixedAngle">If the TargetType is fixed angle, which angle should be used, in degrees.</param>
        /// <param name="dashDamageRadius">Within how many times does a player need to be to be damaged by this dash.</param>
        public Dash(
            float radius = 8f,
            int numDashes = 1,
            float dashSpeed = 1f,
            float dashRange = 4f,
            int cooldownMs = 1000,
            int cooldownOffsetMs = 0,
            int cycleCooldownMs = 1000,
            float angleOffset = 0f,
            int damage = 0,
            Ease ease = Ease.None,
            TargetType targetType = TargetType.ClosestPlayer,
            float fixedAngle = 0f,
            float dashDamageRadius = 0.8f)
        {
            acquireRadiusSqr = radius * radius;
            this.numDashes = numDashes;
            dashTime = MathF.Abs(dashRange / dashSpeed);
            dashTimeMs = (int)(dashTime * 1000);
            this.dashRange = dashRange;
            cooldownMS = cooldownMs;
            cooldownOffsetMS = cooldownOffsetMs;
            cycleCooldownMS = cycleCooldownMs;
            this.angleOffset = angleOffset.Deg2Rad();
            this.damage = damage;
            this.ease = ease;
            this.targetType = targetType;
            this.fixedAngle = fixedAngle.Deg2Rad();
            this.dashDamageRadius = dashDamageRadius;
        }

        public Dash(XElement xml)
        {
            var acquireRadius = xml.GetAttribute<float>("acquireRadius", 8f);
            acquireRadiusSqr = acquireRadius * acquireRadius;
            numDashes = xml.GetAttribute<int>("numDashes", 1);
            dashRange = xml.GetAttribute<float>("dashRange", 4f);
            var dashSpeed = xml.GetAttribute<float>("dashSpeed", 1f);
            dashTime = MathF.Abs(dashRange / dashSpeed);
            dashTimeMs = (int)(dashTime * 1000);
            cooldownMS = xml.GetAttribute<int>("cooldownMS", 1000);
            cooldownOffsetMS = xml.GetAttribute<int>("cooldownOffsetMS", 0);
            cycleCooldownMS = xml.GetAttribute<int>("cycleCooldownMS", 1000);
            angleOffset = xml.GetAttribute<float>("angleOffset", 0f).Deg2Rad();
            damage = xml.GetAttribute<int>("damage", 0);
            ease = Enum.Parse<Ease>(xml.GetAttribute<string>("ease", "None"));
            targetType = Enum.Parse<TargetType>(xml.GetAttribute<string>("targetType", "ClosestPlayer"));
            fixedAngle = xml.GetAttribute<float>("fixedAngle", 0f).Deg2Rad();
            dashDamageRadius = xml.GetAttribute<float>("dashDamageRadius", 0.8f);
        }

        /// <inheritdoc/>
        public override void Start(Character host)
        {
            var dashInfo = host.ResolveResource<DashInfo>(this);
            dashInfo.DashCooldown = cooldownOffsetMS == 0 ? cooldownMS : cooldownOffsetMS;
            dashInfo.DashCount = 0;
            dashInfo.CycleCooldown = 0;
            dashInfo.Dashing = false;
            dashInfo.InCycle = false;
        }

        /// <inheritdoc/>
        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var dashInfo = host.ResolveResource<DashInfo>(this);
            if (dashInfo.CycleCooldown > 0)
            {
                dashInfo.CycleCooldown -= time.ElapsedMsDelta;
                if (dashInfo.CycleCooldown > 0)
                {
                    return BehaviorTickState.OnCooldown;
                }
            }

            if (dashInfo.Dashing)
            {
                var elapsedTimePerc = (time.TotalElapsedMs - dashInfo.DashStarted) / 1000f / dashTime;
                if (ease != Ease.None)
                {
                    Easing.EaseVal(ease, ref elapsedTimePerc);
                }

                var dist = dashRange * elapsedTimePerc;
                var relMovePos = new Vector2(MathF.Cos(dashInfo.DashAngle) * dist, MathF.Sin(dashInfo.DashAngle) * dist);
                host.Move(dashInfo.DashStartPos + relMovePos);
                if (damage != 0)
                {
                    foreach (var plr in host.GetPlayersWithin(dashDamageRadius))
                    {
                        if (dashInfo.HitThisDash.Add(plr))
                        {
                            plr.DamageWithText(damage, from: host);
                        }
                    }
                }

                dashInfo.DashCooldown -= time.ElapsedMsDelta;
                if (dashInfo.DashCooldown <= 0)
                {
                    dashInfo.DashCount = (dashInfo.DashCount + 1) % numDashes;
                    dashInfo.Dashing = false;
                    if (dashInfo.DashCount == 0)
                    {
                        dashInfo.InCycle = false;
                        dashInfo.CycleCooldown = cycleCooldownMS;
                    }
                    else
                    {
                        dashInfo.DashCooldown = cooldownMS;
                    }

                    return BehaviorTickState.BehaviorDeactivate;
                }

                if (!dashInfo.DashStartSent)
                {
                    dashInfo.DashStartSent = true;
                    return BehaviorTickState.BehaviorActivate;
                }

                return BehaviorTickState.BehaviorActive;
            }
            else
            {
                dashInfo.DashCooldown -= time.ElapsedMsDelta;
                if (dashInfo.DashCooldown < 0)
                {
                    dashInfo.Dashing = true;
                    dashInfo.DashCooldown = dashTimeMs;
                    dashInfo.DashStartPos = host.Position.ToVec2();
                    dashInfo.DashStarted = time.TotalElapsedMs;
                    dashInfo.DashStartSent = false;
                    dashInfo.HitThisDash.Clear();
                    SetTarget(host, dashInfo);
                    if (dashInfo.Dashing == false)
                    {
                        return BehaviorTickState.BehaviorFailed;
                    }

                    dashInfo.InCycle = true;
                    dashInfo.DashAngle += angleOffset;
                    return BehaviorTickState.BehaviorActive;
                }

                return BehaviorTickState.OnCooldown;
            }
        }

        private void SetTarget(Character host, DashInfo dashInfo)
        {
            switch (targetType)
            {
                case TargetType.ClosestPlayer:
                case TargetType.RandomPlayerPerBehavior:
                case TargetType.FarthestPlayer:
                    var target = host.GetAttackTarget(acquireRadiusSqr, targetType);
                    if (target == null)
                    {
                        dashInfo.Dashing = false;
                        dashInfo.DashCooldown = cooldownMS;
                        return;
                    }

                    dashInfo.DashAngle = host.GetAngleBetween(target);
                    break;
                case TargetType.RandomPlayerPerCycle:
                    target = dashInfo.InCycle ? host.World.GetPlayerById(dashInfo.CurrentTargetID) : host.GetAttackTarget(acquireRadiusSqr, targetType);
                    if (target == null)
                    {
                        dashInfo.Dashing = false;
                        dashInfo.DashCooldown = cooldownMS;
                        dashInfo.CurrentTargetID = -1;
                        return;
                    }

                    dashInfo.CurrentTargetID = target.Id;
                    dashInfo.DashAngle = host.GetAngleBetween(target);
                    break;
                case TargetType.FixedAngle:
                    dashInfo.DashAngle = fixedAngle;
                    break;
            }
        }
    }
}
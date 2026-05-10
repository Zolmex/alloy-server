using System;
using System.Numerics;
using System.Xml.Linq;
using Common.Game;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Entities.Behaviors.Actions.Info;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record Dash : BehaviorScript {
    private readonly float acquireRadiusSqr;
    private readonly float angleOffset;
    private readonly int cooldownMS;
    private readonly int cooldownOffsetMS;
    private readonly int cycleCooldownMS;
    private readonly int damage;
    private readonly float dashDamageRadius;
    private readonly float dashRange;
    private readonly float dashTime;
    private readonly int dashTimeMs;
    private readonly Ease ease;
    private readonly float fixedAngle;
    private readonly int numDashes;
    private readonly TargetType targetType;

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
        float dashDamageRadius = 0.8f) {
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

    public override void Start(ref EntityView host) {
        var dashInfo = host.Behavior.Resources.ResolveResource<DashInfo>(this);
        dashInfo.DashCooldown = cooldownOffsetMS == 0 ? cooldownMS : cooldownOffsetMS;
        dashInfo.DashCount = 0;
        dashInfo.CycleCooldown = 0;
        dashInfo.Dashing = false;
        dashInfo.InCycle = false;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var dashInfo = host.Behavior.Resources.ResolveResource<DashInfo>(this);
        if (dashInfo.CycleCooldown > 0) {
            dashInfo.CycleCooldown -= time.ElapsedMsDelta;
            if (dashInfo.CycleCooldown > 0) return BehaviorTickState.OnCooldown;
        }

        if (dashInfo.Dashing) {
            var elapsedTimePerc = (time.TotalElapsedMs - dashInfo.DashStarted) / 1000f / dashTime;
            if (ease != Ease.None) Easing.EaseVal(ease, ref elapsedTimePerc);

            var dist = dashRange * elapsedTimePerc;
            var relMovePos = new Vector2(MathF.Cos(dashInfo.DashAngle) * dist, MathF.Sin(dashInfo.DashAngle) * dist);
            host.Stats.Move(dashInfo.DashStartPos + relMovePos);
            if (damage != 0)
                foreach (var plrId in host.World.Map.GetPlayersWithin(host.Stats.Pos, dashDamageRadius)) {
                    ref var plr = ref host.World.EntityCombat.Get(plrId);
                    if (dashInfo.HitThisDash.Add(plrId))
                        plr.DamageWithText(damage);
                }

            dashInfo.DashCooldown -= time.ElapsedMsDelta;
            if (dashInfo.DashCooldown <= 0) {
                dashInfo.DashCount = (dashInfo.DashCount + 1) % numDashes;
                dashInfo.Dashing = false;
                if (dashInfo.DashCount == 0) {
                    dashInfo.InCycle = false;
                    dashInfo.CycleCooldown = cycleCooldownMS;
                }
                else {
                    dashInfo.DashCooldown = cooldownMS;
                }

                return BehaviorTickState.BehaviorDeactivate;
            }

            if (!dashInfo.DashStartSent) {
                dashInfo.DashStartSent = true;
                return BehaviorTickState.BehaviorActivate;
            }

            return BehaviorTickState.BehaviorActive;
        }

        dashInfo.DashCooldown -= time.ElapsedMsDelta;
        if (dashInfo.DashCooldown < 0) {
            dashInfo.Dashing = true;
            dashInfo.DashCooldown = dashTimeMs;
            dashInfo.DashStartPos = host.Stats.Pos.ToVec2();
            dashInfo.DashStarted = time.TotalElapsedMs;
            dashInfo.DashStartSent = false;
            dashInfo.HitThisDash.Clear();
            SetTarget(ref host, dashInfo);
            if (!dashInfo.Dashing) return BehaviorTickState.BehaviorFailed;

            dashInfo.InCycle = true;
            dashInfo.DashAngle += angleOffset;
            return BehaviorTickState.BehaviorActive;
        }

        return BehaviorTickState.OnCooldown;
    }

    private void SetTarget(ref EntityView host, DashInfo dashInfo) {
        switch (targetType) {
            case TargetType.ClosestPlayer:
            case TargetType.RandomPlayerPerBehavior:
            case TargetType.FarthestPlayer:
                var targetId = host.World.GetAttackTarget(host.Stats.Pos, acquireRadiusSqr, targetType);
                if (targetId == 0) {
                    dashInfo.Dashing = false;
                    dashInfo.DashCooldown = cooldownMS;
                    return;
                }

                ref var target = ref host.World.EntityStats.Get(targetId);
                dashInfo.DashAngle = host.Stats.GetAngleBetween(ref target);
                break;
            case TargetType.RandomPlayerPerCycle:
                targetId = dashInfo.InCycle
                    ? dashInfo.CurrentTargetID
                    : host.World.GetAttackTarget(host.Stats.Pos, acquireRadiusSqr, targetType);
                if (targetId == 0) {
                    dashInfo.Dashing = false;
                    dashInfo.DashCooldown = cooldownMS;
                    dashInfo.CurrentTargetID = -1;
                    return;
                }

                target = ref host.World.EntityStats.Get(targetId);
                dashInfo.CurrentTargetID = target.Id;
                dashInfo.DashAngle = host.Stats.GetAngleBetween(ref target);
                break;
            case TargetType.FixedAngle:
                dashInfo.DashAngle = fixedAngle;
                break;
        }
    }
}
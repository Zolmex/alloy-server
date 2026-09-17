using System;
using System.Numerics;
using System.Xml.Linq;
using Arch.Core;
using Common.Game;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Behaviors.Actions.Info;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Systems;
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

    public override void Start(ref EntityContext host) {
        var dashInfo = host.BehavController.Resources.ResolveResource<DashInfo>(this);
        dashInfo.DashCooldown = cooldownOffsetMS == 0 ? cooldownMS : cooldownOffsetMS;
        dashInfo.DashCount = 0;
        dashInfo.CycleCooldown = 0;
        dashInfo.Dashing = false;
        dashInfo.InCycle = false;
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var dashInfo = host.BehavController.Resources.ResolveResource<DashInfo>(this);
        if (dashInfo.CycleCooldown > 0) {
            dashInfo.CycleCooldown -= time.ElapsedMsDelta;
            if (dashInfo.CycleCooldown > 0) return BehaviorTickState.OnCooldown;
        }

        var w = host.World;
        ref var hostPos = ref host.Position;
        if (dashInfo.Dashing) {
            var elapsedTimePerc = (time.TotalElapsedMs - dashInfo.DashStarted) / 1000f / dashTime;
            if (ease != Ease.None) Easing.EaseVal(ease, ref elapsedTimePerc);

            var dist = dashRange * elapsedTimePerc;
            var relMovePos = new Vector2(MathF.Cos(dashInfo.DashAngle) * dist, MathF.Sin(dashInfo.DashAngle) * dist);
            hostPos.Move(dashInfo.DashStartPos + relMovePos);
            if (damage != 0)
                foreach (var player in w.Map.GetPlayersWithin(hostPos.Pos, dashDamageRadius)) {
                    if (dashInfo.HitThisDash.Add(player))
                        w.DamageSystem.DamageWithText(new DamageRecord(host.Entity, player, damage));
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
            dashInfo.DashStartPos = hostPos.Pos.ToVec2();
            dashInfo.DashStarted = time.TotalElapsedMs;
            dashInfo.DashStartSent = false;
            dashInfo.HitThisDash.Clear();
            SetTarget(host, dashInfo, ref hostPos);
            if (!dashInfo.Dashing) return BehaviorTickState.BehaviorFailed;

            dashInfo.InCycle = true;
            dashInfo.DashAngle += angleOffset;
            return BehaviorTickState.BehaviorActive;
        }

        return BehaviorTickState.OnCooldown;
    }

    private void SetTarget(EntityContext host, DashInfo dashInfo, ref Position hostPos) {
        
        switch (targetType) {
            case TargetType.ClosestPlayer:
            case TargetType.RandomPlayerPerBehavior:
            case TargetType.FarthestPlayer:
                var target = host.World.GetAttackTarget(hostPos.Pos, acquireRadiusSqr, targetType);
                if (target == Entity.Null) {
                    dashInfo.Dashing = false;
                    dashInfo.DashCooldown = cooldownMS;
                    return;
                }

                ref var targetPos = ref host.World.Ecs.Get<Position>(target);
                dashInfo.DashAngle = hostPos.GetAngleBetween(targetPos.Pos);
                break;
            case TargetType.RandomPlayerPerCycle:
                target = dashInfo.InCycle
                    ? dashInfo.CurrentTarget
                    : host.World.GetAttackTarget(hostPos.Pos, acquireRadiusSqr, targetType);
                if (target == Entity.Null) {
                    dashInfo.Dashing = false;
                    dashInfo.DashCooldown = cooldownMS;
                    dashInfo.CurrentTarget = Entity.Null;
                    return;
                }

                targetPos = ref host.World.Ecs.Get<Position>(target);
                dashInfo.CurrentTarget = target;
                dashInfo.DashAngle = hostPos.GetAngleBetween(targetPos.Pos);
                break;
            case TargetType.FixedAngle:
                dashInfo.DashAngle = fixedAngle;
                break;
        }
    }
}
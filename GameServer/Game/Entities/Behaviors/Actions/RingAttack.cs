using System;
using Common;
using Common.Game;
using Common.Projectiles.ProjectilePaths;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Extensions;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class RingAttackInfo {
    public float AngleToIncrement;
    public int CoolDownLeft;
    public float FixedAngle;
    public float SavedAngle;
    public bool Targeted;
}

public record RingAttack : BehaviorScript {
    private readonly float _angleOffset;
    private readonly float _angleToIncrement;
    private readonly int _coolDownMS;
    private readonly int _count;
    private readonly float _fixedAngle;
    private readonly byte _projectileIndex;
    private readonly float _radius;
    private readonly bool _seeInvis;
    private readonly bool _targeted;
    private readonly bool _useSavedAngle;

    public RingAttack(float radius, int count, float offset, byte projectileIndex, float angleToIncrement,
        float fixedAngle = 0, bool targeted = false, int coolDownMS = 0, bool seeInvis = false,
        bool useSavedAngle = false) {
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

    public override void Start(ref EntityView host) {
        var state = host.Behavior.Resources.ResolveResource<RingAttackInfo>(this);
        state.AngleToIncrement = _angleToIncrement;
        state.FixedAngle = _fixedAngle;
        state.CoolDownLeft = _coolDownMS;
        state.Targeted = _targeted;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var state = host.Behavior.Resources.ResolveResource<RingAttackInfo>(this);

        // if (host.HasConditionEffect(ConditionEffectIndex.Stunned)) // TODO: condition effects
        //     return BehaviorTickState.BehaviorFailed;

        if (state.CoolDownLeft > 0) {
            state.CoolDownLeft -= time.ElapsedMsDelta;
            return BehaviorTickState.OnCooldown;
        }

        var entityId = _radius == 0 ? EntityId.Null : host.World.Map.GetNearestOtherEntityByName(host.Stats.Pos, host.Id, null, _radius);
        ref var entity = ref host.World.EntityStats.Get(entityId);
        var angleInc = 2 * MathF.PI / _count;
        var projProps = host.Entity.Desc.Projectiles[_projectileIndex].Props;

        float angle = 0;
        if (state.Targeted) {
            if (state.AngleToIncrement != 0) {
                if (_useSavedAngle)
                    state.FixedAngle = state.SavedAngle;

                state.FixedAngle += state.AngleToIncrement;
                state.SavedAngle = state.FixedAngle;
            }

            angle = state.FixedAngle;
        }
        else {
            angle = entityId == EntityId.Null
                ? _angleOffset
                : (float)Math.Atan2(entity.Pos.Y - host.Stats.Pos.Y, entity.Pos.X - host.Stats.Pos.X) +
                  _angleOffset;
        }

        var count = _count;
        // if (host.HasConditionEffect(ConditionEffectIndex.Dazed)) // TODO: condition effects
        //     count = Math.Max(1, count / 2);

        var dmg = host.Combat.GetProjectileDamage(projProps.MinDamage, projProps.MaxDamage);
        var startAngle = angle * (count - 1) / 2;
        var path = ProjectilePathSegment.ParsePath(projProps).ToPath();
        host.World.EnemyShootProjectiles(host.Stats.Pos, host.Id, 
            _projectileIndex, startAngle.Rad2Deg(), dmg, (byte)count,
            angleInc.Rad2Deg(), path, path.LifetimeMs, projProps.MultiHit, ref time);

        state.CoolDownLeft = time.ElapsedMsDelta;
        return BehaviorTickState.BehaviorActive;
    }
}
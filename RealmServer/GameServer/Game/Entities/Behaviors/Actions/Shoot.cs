#region

using Common;
using Common.ProjectilePaths;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using System;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class ShootInfo
    {
        public int CooldownLeft;
        public float AngleOffset;
        public ProjectileProps Props;
        public ProjectilePathSegment Path;
    }

    public record Shoot : BehaviorScript
    {
        private const int PREDICT_NUM_TICKS = 10;

        private static readonly Logger _log = new(typeof(Shoot));
        private static readonly ProjectileProps _bladeProps = new(0, "Blade", 1000, 10, 0);

        private readonly float minRadiusSqr;
        private readonly float maxRadiusSqr;
        private readonly byte count;
        private readonly float shootAngle;
        private readonly ushort projType;
        private readonly int lifetimeMs;
        private readonly int minDamage;
        private readonly int maxDamage;
        private readonly float fixedAngle;
        private readonly float angleOffsetDefault;
        private readonly float xOffset;
        private readonly float yOffset;
        private readonly float predictive;
        private readonly int cooldownOffsetMs;
        private readonly int cooldownMs;
        private readonly float rotateAngle;
        private readonly TargetType targetType;
        private readonly ProjectilePath path;
        private Action<Character, Character> onHitEvent; // hit, hit by
        private readonly int size;
        private readonly int propId = -1;
        private readonly bool multiHit;
        private readonly bool passesCover;
        private readonly bool armorPiercing;
        private readonly (ConditionEffectIndex, int)[] effects;

        public Shoot(float maxRadius, byte count = 1, float shootAngle = 0f, int projectileIndex = 0,
            float fixedAngle = 0f, float rotateAngle = 0f, float angleOffset = 0f,
            float predictive = 0f, int coolDownOffset = 0, int cooldownMS = 0, bool targeted = false,
            float xOffset = 0f, float yOffset = 0f, Action<Character, Character> onHitEvent = null, float minRadius = 0)
        {
            propId = projectileIndex;
            maxRadiusSqr = maxRadius * maxRadius;
            minRadiusSqr = minRadius * minRadius;
            this.count = count;
            this.shootAngle = shootAngle.Deg2Rad();
            this.fixedAngle = fixedAngle.Deg2Rad();
            this.rotateAngle = rotateAngle.Deg2Rad();
            angleOffsetDefault = angleOffset.Deg2Rad();
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.predictive = predictive;
            cooldownOffsetMs = coolDownOffset;
            cooldownMs = cooldownMS;
            targetType = targeted ? TargetType.ClosestPlayer : TargetType.FixedAngle;
            this.onHitEvent = onHitEvent;
        }

        public Shoot(float maxRadius, ProjectilePathSegment path, byte count = 1, float shootAngle = 0f,
            ushort projType = 0, float fixedAngle = 0f, float rotateAngle = 0f, float angleOffset = 0f,
            float predictive = 0f, int coolDownOffset = 0, int cooldownMS = 0, bool targeted = false,
            string projName = "",
            int lifetimeMs = 1000, int minDamage = -1, int maxDamage = -1, int damage = -1, float xOffset = 0f,
            float yOffset = 0f, Action<Character, Character> onHitEvent = null,
            int size = 100, bool multiHit = false, bool passesCover = false, bool armorPiercing = false,
            float minRadius = 0,
            params (ConditionEffectIndex, int)[] effects)
        {
            maxRadiusSqr = maxRadius * maxRadius;
            minRadiusSqr = minRadius * minRadius;
            this.count = count;
            this.shootAngle = shootAngle.Deg2Rad();
            if (string.IsNullOrEmpty(projName))
                this.projType = projType;
            else
            {
                var projXml = XmlLibrary.Id2Object(projName);
                if (projXml == null)
                {
                    projXml = XmlLibrary.Id2Object("Blade");
                    _log.Warn($"Projectile {projName} not found. Using Blade instead.");
                }

                this.projType = projXml.ObjectType;
            }

            this.lifetimeMs = lifetimeMs;
            if (damage == -1)
            {
                this.minDamage = minDamage;
                this.maxDamage = maxDamage;
            }
            else
            {
                this.minDamage = damage;
                this.maxDamage = damage;
            }

            this.fixedAngle = fixedAngle.Deg2Rad();
            this.rotateAngle = rotateAngle.Deg2Rad();
            angleOffsetDefault = angleOffset.Deg2Rad();
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.predictive = predictive;
            cooldownOffsetMs = coolDownOffset;
            cooldownMs = cooldownMS;
            targetType = targeted ? TargetType.ClosestPlayer : TargetType.FixedAngle;
            path.LifetimeMs = lifetimeMs;
            this.path = path.ToPath();
            this.onHitEvent = onHitEvent;
            this.size = size;
            this.multiHit = multiHit;
            this.passesCover = passesCover;
            this.armorPiercing = armorPiercing;
            this.effects = effects;
        }

        public Shoot(float maxRadius, ProjectilePath path, byte count = 1, float shootAngle = 0f,
            ushort projType = 0, float fixedAngle = 0f, float rotateAngle = 0f, float angleOffset = 0f,
            float predictive = 0f, int coolDownOffset = 0, int cooldownMS = 0, bool targeted = false,
            string projName = "", int minDamage = -1, int maxDamage = -1, int damage = -1, float xOffset = 0f,
            float yOffset = 0f, Action<Character, Character> onHitEvent = null,
            int size = 100, bool multiHit = false, bool passesCover = false, bool armorPiercing = false,
            float minRadius = 0,
            params (ConditionEffectIndex, int)[] effects)
        {
            maxRadiusSqr = maxRadius * maxRadius;
            minRadiusSqr = minRadius * minRadius;
            this.count = count;
            this.shootAngle = shootAngle.Deg2Rad();
            if (string.IsNullOrEmpty(projName))
                this.projType = projType;
            else
            {
                var projXml = XmlLibrary.Id2Object(projName);
                if (projXml == null)
                {
                    projXml = XmlLibrary.Id2Object("Blade");
                    _log.Warn($"Projectile {projName} not found. Using Blade instead.");
                }

                this.projType = projXml.ObjectType;
            }

            if (damage == -1)
            {
                this.minDamage = minDamage;
                this.maxDamage = maxDamage;
            }
            else
            {
                this.minDamage = damage;
                this.maxDamage = damage;
            }

            this.fixedAngle = fixedAngle.Deg2Rad();
            this.rotateAngle = rotateAngle.Deg2Rad();
            angleOffsetDefault = angleOffset.Deg2Rad();
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.predictive = predictive;
            cooldownOffsetMs = coolDownOffset;
            cooldownMs = cooldownMS;
            targetType = targeted ? TargetType.ClosestPlayer : TargetType.FixedAngle;
            this.path = path;
            lifetimeMs = path.LifetimeMs;
            this.onHitEvent = onHitEvent;
            this.size = size;
            this.multiHit = multiHit;
            this.passesCover = passesCover;
            this.armorPiercing = armorPiercing;
            this.effects = effects;
        }

        public Shoot(float maxRadius, ProjectilePath path, byte count = 1, float shootAngle = 0f,
            ushort projType = 0, float fixedAngle = 0f, float rotateAngle = 0f, float angleOffset = 0f,
            float predictive = 0f, int coolDownOffset = 0, int cooldownMS = 0,
            TargetType targetType = TargetType.ClosestPlayer, string projName = "",
            int lifetimeMs = 1000, int minDamage = -1, int maxDamage = -1, int damage = -1, float xOffset = 0f,
            float yOffset = 0f, Action<Character, Character> onHitEvent = null,
            int size = 100, bool multiHit = false, bool passesCover = false, bool armorPiercing = false,
            float minRadius = 0,
            params (ConditionEffectIndex, int)[] effects)
        {
            maxRadiusSqr = maxRadius * maxRadius;
            minRadiusSqr = minRadius * minRadius;
            this.count = count;
            this.shootAngle = shootAngle.Deg2Rad();
            if (string.IsNullOrEmpty(projName))
                this.projType = projType;
            else
                this.projType = XmlLibrary.Id2Object(projName).ObjectType;
            if (damage == -1)
            {
                this.minDamage = minDamage;
                this.maxDamage = maxDamage;
            }
            else
            {
                this.minDamage = damage;
                this.maxDamage = damage;
            }

            this.fixedAngle = fixedAngle.Deg2Rad();
            this.rotateAngle = rotateAngle.Deg2Rad();
            angleOffsetDefault = angleOffset.Deg2Rad();
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.predictive = predictive;
            cooldownOffsetMs = coolDownOffset;
            cooldownMs = cooldownMS;
            this.targetType = targetType;
            this.path = path;
            this.lifetimeMs = path.LifetimeMs;
            this.onHitEvent = onHitEvent;
            this.size = size;
            this.multiHit = multiHit;
            this.passesCover = passesCover;
            this.armorPiercing = armorPiercing;
            this.effects = effects;
        }

        public Shoot(float maxRadius, ProjectilePathSegment path, byte count = 1, float shootAngle = 0f,
            ushort projType = 0, float fixedAngle = 0f, float rotateAngle = 0f, float angleOffset = 0f,
            float predictive = 0f, int coolDownOffset = 0, int cooldownMS = 0,
            TargetType targetType = TargetType.ClosestPlayer, string projName = "",
            int lifetimeMs = 1000, int minDamage = -1, int maxDamage = -1, int damage = -1, float xOffset = 0f,
            float yOffset = 0f, Action<Character, Character> onHitEvent = null,
            int size = 100, bool multiHit = false, bool passesCover = false, bool armorPiercing = false,
            float minRadius = 0,
            params (ConditionEffectIndex, int)[] effects)
        {
            maxRadiusSqr = maxRadius * maxRadius;
            minRadiusSqr = minRadius * minRadius;
            this.count = count;
            this.shootAngle = shootAngle.Deg2Rad();
            if (string.IsNullOrEmpty(projName))
                this.projType = projType;
            else
                this.projType = XmlLibrary.Id2Object(projName).ObjectType;
            this.lifetimeMs = lifetimeMs;
            if (damage == -1)
            {
                this.minDamage = minDamage;
                this.maxDamage = maxDamage;
            }
            else
            {
                this.minDamage = damage;
                this.maxDamage = damage;
            }

            this.fixedAngle = fixedAngle.Deg2Rad();
            this.rotateAngle = rotateAngle.Deg2Rad();
            angleOffsetDefault = angleOffset.Deg2Rad();
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.predictive = predictive;
            cooldownOffsetMs = coolDownOffset;
            cooldownMs = cooldownMS;
            this.targetType = targetType;
            path.LifetimeMs = lifetimeMs;
            this.path = path.ToPath();
            this.onHitEvent = onHitEvent;
            this.size = size;
            this.multiHit = multiHit;
            this.passesCover = passesCover;
            this.armorPiercing = armorPiercing;
            this.effects = effects;
        }

        public Shoot(float maxRadius, ShootConfig config, ProjectilePath path, float minRadius = 0)
        {
            maxRadiusSqr = maxRadius * maxRadius;
            minRadiusSqr = minRadius * minRadius;
            count = config.Count;
            shootAngle = config.ShootAngle.Deg2Rad();
            if (string.IsNullOrEmpty(config.ProjName)) projType = config.ProjType;
            else projType = XmlLibrary.Id2Object(config.ProjName).ObjectType;
            fixedAngle = config.FixedAngle.Deg2Rad();
            rotateAngle = config.RotateAngle.Deg2Rad();
            angleOffsetDefault = config.AngleOffset.Deg2Rad();
            predictive = config.Predictive;
            cooldownOffsetMs = config.CooldownOffsetMs;
            cooldownMs = config.CooldownMs;
            targetType = config.TargetType;
            lifetimeMs = path.LifetimeMs;
            if (config.Damage == -1)
            {
                minDamage = config.MinDamage;
                maxDamage = config.MaxDamage;
            }
            else
            {
                minDamage = config.Damage;
                maxDamage = config.Damage;
            }

            xOffset = config.XOffset;
            yOffset = config.YOffset;
            onHitEvent = config.OnHitEvent;
            size = config.Size;
            multiHit = config.MultiHit;
            passesCover = config.PassesCover;
            armorPiercing = config.ArmorPiercing;
            effects = config.Effects;
            this.path = path;
        }

        public override void Start(Character host)
        {
            var shootInfo = host.ResolveResource<ShootInfo>(this);
            shootInfo.CooldownLeft = cooldownOffsetMs;
            shootInfo.AngleOffset = 0;
            if (propId != -1)
            {
                if (!host.Desc.Projectiles.TryGetValue((byte)propId, out var props))
                {
                    // Try to find the projectile property id
                    shootInfo.Props = _bladeProps;
                    _log.Error($"(ObjId {host.Desc.ObjectId}) projectile id {propId} not found. Using Blade instead.");
                }
                else
                {
                    shootInfo.Props = props;
                }

                var projXml = XmlLibrary.Id2Object(shootInfo.Props.ObjectId); // Find the projectile xml
                if (projXml == null)
                {
                    projXml = XmlLibrary.Id2Object("Blade");
                    _log.Warn($"Projectile {shootInfo.Props.ObjectId} not found. Using Blade instead.");
                }

                shootInfo.Props.ObjectType = projXml.ObjectType;
                shootInfo.Path = ProjectilePathSegment.ParsePath(shootInfo.Props);
            }
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var shootInfo = host.ResolveResource<ShootInfo>(this);
            if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
                return BehaviorTickState.BehaviorFailed;

            if (shootInfo.CooldownLeft > 0)
            {
                shootInfo.CooldownLeft -= time.ElapsedMsDelta;
                if (shootInfo.CooldownLeft > 0)
                    return BehaviorTickState.OnCooldown;
            }

            var startAngle = fixedAngle;
            if (targetType != TargetType.FixedAngle)
            {
                var attackTarget = host.GetAttackTarget(maxRadiusSqr, targetType, minRadiusSqr);
                if (attackTarget == null)
                    return BehaviorTickState.BehaviorFailed;

                if (predictive > 0)
                    startAngle = Predict(host, attackTarget);
                else
                    startAngle = (float)Math.Atan2(attackTarget.Position.Y - host.Position.Y,
                        attackTarget.Position.X - host.Position.X);
                startAngle -= ((count / 2f) - 0.5f) * shootAngle;
            }

            startAngle += angleOffsetDefault;
            if (rotateAngle != 0)
            {
                startAngle += shootInfo.AngleOffset;
                shootInfo.AngleOffset += rotateAngle;
            }

            if (propId == -1)
            {
                host.ShootProjectiles(path, projType, minDamage, maxDamage, count, startAngle.Rad2Deg(),
                    host.Position.X + xOffset, host.Position.Y + yOffset, shootAngle.Rad2Deg(), multiHit, passesCover,
                    armorPiercing, onHitEvent,
                    size, effects, radiusSqr: maxRadiusSqr);
            }
            else
            {
                host.ShootProjectiles(shootInfo.Path.ToPath(), shootInfo.Props.ObjectType, shootInfo.Props.MinDamage,
                    shootInfo.Props.MaxDamage, count, startAngle.Rad2Deg(),
                    host.Position.X + xOffset, host.Position.Y + yOffset, shootAngle.Rad2Deg(),
                    shootInfo.Props.MultiHit, shootInfo.Props.PassesCover, shootInfo.Props.ArmorPiercing,
                    onHitEvent, shootInfo.Props.Size, shootInfo.Props.Effects, propId, maxRadiusSqr);
            }

            shootInfo.CooldownLeft = cooldownMs;
            return BehaviorTickState.BehaviorActive;
        }

        private static float Predict(Character host, Entity target)
        {
            var targetX = target.Position.X + (PREDICT_NUM_TICKS * (target.Position.X - target.PrevPosition.X));
            var targetY = target.Position.Y + (PREDICT_NUM_TICKS * (target.Position.Y - target.PrevPosition.Y));
            var angle = MathF.Atan2(targetY - host.Position.Y, targetX - host.Position.X);
            return angle;
        }
    }
}
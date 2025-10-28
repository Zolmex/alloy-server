#region

using Common;
using System;
using static GameServer.Game.Entities.Behaviors.BehaviorScript;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    /// <summary>
    /// Configuration class for the Shoot constructor.
    /// </summary>
    public class ShootConfig
    {
        /// <summary>
        /// Gets or sets the number of projectiles to shoot.
        /// </summary>
        public byte Count { get; set; } = 1;

        /// <summary>
        /// Gets or sets the angle at which the projectile is shot.
        /// </summary>
        public float ShootAngle { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the type of the projectile.
        /// </summary>
        public ushort ProjType { get; set; } = 0;

        /// <summary>
        /// Gets or sets the name of the projectile.
        /// </summary>
        public string ProjName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the fixed angle of the projectile.
        /// </summary>
        public float FixedAngle { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the rotation angle of the projectile.
        /// </summary>
        public float RotateAngle { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the angle offset of the projectile.
        /// </summary>
        public float AngleOffset { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the predictive aiming value.
        /// </summary>
        public float Predictive { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the cooldown offset in milliseconds.
        /// </summary>
        public int CooldownOffsetMs { get; set; } = 0;

        /// <summary>
        /// Gets or sets the cooldown duration in milliseconds.
        /// </summary>
        public int CooldownMs { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the projectile is targeted.
        /// </summary>
        public TargetType TargetType { get; set; } = TargetType.ClosestPlayer;

        /// <summary>
        /// Gets or sets the minimum damage of the projectile.
        /// </summary>
        public int MinDamage { get; set; } = -1;

        /// <summary>
        /// Gets or sets the maximum damage of the projectile.
        /// </summary>
        public int MaxDamage { get; set; } = -1;

        /// <summary>
        /// Gets or sets the damage of the projectile.
        /// </summary>
        public int Damage { get; set; } = -1;

        /// <summary>
        /// Gets or sets the X offset of the projectile.
        /// </summary>
        public float XOffset { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the Y offset of the projectile.
        /// </summary>
        public float YOffset { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the event triggered when the projectile hits a target.
        /// </summary>
        public Action<Character, Character> OnHitEvent { get; set; } = null;

        /// <summary>
        /// Gets or sets the size of the projectile.
        /// </summary>
        public int Size { get; set; } = 100;

        /// <summary>
        /// Gets or sets a value indicating whether the projectile can hit multiple targets.
        /// </summary>
        public bool MultiHit { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the projectile can pass through cover.
        /// </summary>
        public bool PassesCover { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the projectile is armor-piercing.
        /// </summary>
        public bool ArmorPiercing { get; set; } = false;

        /// <summary>
        /// Gets or sets the condition effects applied by the projectile.
        /// </summary>
        public (ConditionEffectIndex, int)[] Effects { get; set; } = Array.Empty<(ConditionEffectIndex, int)>();
    }
}
#region

using Common;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.DamageSources.Projectiles;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        private const float MIN_ATTACK_MULT = 0.5f;
        private const float MAX_ATTACK_MULT = 2f;

        public Character LastHitTarget;
        public Character DamageCounterTarget;

        public bool ShotsVisible(Player player)
        {
            switch (User.GameInfo.AllyShots)
            {
                case 0:
                    return true; // All
                case 1:
                    return false; // None
                case 2:
                    return false; // Locked
                case 3:
                    return false; // Guild
                case 4:
                    return false; // Both
                default:
                    return true;
            }
        }

        public void Shoot(ProjectileDesc projDesc, byte numShots, float attackAngle, float arcGap, Vector2 startPos,
            int[] dmgList = null, float[] critList = null)
        {
            var totalArc = arcGap * (numShots - 1);
            var angle = attackAngle - (totalArc / 2);
            for (var i = 0; i < numShots; i++)
            {
                var dmg = 0;
                var crit = 0f;
                if (dmgList == null)
                {
                    var dmgs = GetProjDamage(projDesc);
                    dmg = (int)dmgs[0];
                    crit = dmgs[1];
                }
                else
                {
                    dmg = dmgList[i];
                    crit = critList[i];
                }

                ushort projIndex;
                using (TimedLock.Lock(_projIdLock))
                    projIndex = _nextProjectileId++;

                var proj = new Projectile(this, projIndex, RealmManager.WorldTime.TotalElapsedMs, angle.Rad2Deg(),
                    startPos, (int)(dmg * crit), projDesc.Path.Clone(),
                    projDesc.LifetimeMS, projDesc.MultiHit, projDesc.PassesCover, projDesc.ArmorPiercing,
                    Projectile.ProjectileTargetType.Enemy);
                QueueProjectile(proj);
                OnShoot?.Invoke(proj);
                angle += arcGap;
            }

            OnDoShoot?.Invoke(projDesc, attackAngle.Rad2Deg(), startPos);
        }

        // The client will send back a PlayerShoot packet with the angle and time of the shot, validated by the server in PlayerShoot.Handle()
        public void ServerShoot(ProjectileDesc projDesc, byte numShots, float angle, float angleInc, Vector2 startPos,
            int itemType, int projDmg = 0, float critMult = 1)
        {
            var dmgList = new int[numShots];
            var critList = new float[numShots];
            for (var i = 0; i < numShots; i++) // Calculate damage for each projectile
            {
                var dmgs = projDmg != 0 ? [projDmg, critMult] : GetProjDamage(projDesc);
                var dmg = projDmg != 0 ? projDmg : (int)dmgs[0];
                dmgList[i] = dmg;
                critList[i] = dmgs[1];
            }

            AwaitServerShoot(itemType, startPos, angle, angleInc, dmgList, critList);

            ServerPlayerShoot.Write(User.Network,
                new WorldPosData(startPos.X, startPos.Y),
                angle, // Angle is sent in degrees
                angleInc,
                dmgList,
                critList,
                itemType);
        }

        public float[] GetProjDamage(ProjectileDesc projDesc = null, int minDamage = 0, int maxDamage = 0,
            bool normalRandom = true, bool forceCrit = false)
        {
            var minDmg = projDesc == null ? minDamage : projDesc.MinDamage;
            var maxDmg = projDesc == null ? maxDamage : projDesc.MaxDamage;

            float damage = normalRandom
                ? User.Random.NextIntRange((uint)minDmg, (uint)maxDmg)
                : User.ServerRandom.NextIntRange((uint)minDmg, (uint)maxDmg);

            damage *= GetDamageMultiplier();

            var critMult = GetCritMultiplier(normalRandom, forceCrit);
            damage *= critMult;

            return new float[] { damage, critMult };
        }

        public float GetAttackFrequency()
        {
            if (HasConditionEffect(ConditionEffectIndex.Dazed))
                return 1;

            var freq = AttackSpeed;
            if (HasConditionEffect(ConditionEffectIndex.Berserk))
                freq *= 1.5f;
            return freq;
        }

        private void SendDamageCounterUpdate()
        {
            var target = LastHitTarget;
            
            // Prioritize quest enemy if in range
            var questDist = Quest?.DistSqr(this) ?? float.MaxValue;
            if (questDist <= SIGHT_RADIUS_SQR)
                target = Quest;

            if (target == null || target.Dead || target.World != World)
                return;

            DamageCounterTarget = target;

            var targetId = DamageCounterTarget.Id;
            var plrDamage = DamageCounterTarget.DamageStorage.GetDamageForPlayer(this);
            var topDealers = User.GameInfo.DamageCounter switch
            {
                < 3 => DamageCounterTarget.DamageStorage.GetTopDamageDealers(5),
                _ => new List<KeyValuePair<Player, int>>(),
            };
    
            DamageCounterUpdate.Write(User.Network, targetId, plrDamage, topDealers);
        }
    }
}
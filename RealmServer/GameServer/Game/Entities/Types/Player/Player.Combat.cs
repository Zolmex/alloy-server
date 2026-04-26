#region

using Common;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using System.Collections.Generic;
using System.Numerics;
using GameServer.Game.Entities.DamageSources.Types;

#endregion

namespace GameServer.Game.Entities.Types;

public partial class Player
{
    private const float MIN_ATTACK_MULT = 0.5f;
    private const float MAX_ATTACK_MULT = 2f;
    private const float MIN_ATTACK_FREQ = 0.0015f;
    private const float MAX_ATTACK_FREQ = 0.008f;

    public CharacterEntity DamageCounterTarget;

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
            projIndex = _nextBulletId++;

            var proj = new Projectile(this, projIndex, RealmManager.WorldTime.TotalElapsedMs, angle.Rad2Deg(),
                startPos, (int)(dmg * crit), projDesc.Path.Clone(), Projectile.ProjectileTargetType.Enemy);
            proj.SetProps(projDesc);
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

        User.SendPacket(new ServerPlayerShoot(
            new WorldPosData(startPos.X, startPos.Y),
            angle, // Angle is sent in degrees
            angleInc,
            dmgList,
            critList,
            itemType));
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

        return new[] { damage, critMult };
    }

    public float GetAttackFrequency()
    {
        if (HasConditionEffect(ConditionEffectIndex.Dazed))
            return 1;

        var freq = MIN_ATTACK_FREQ + (Dexterity / 75f) * (MAX_ATTACK_FREQ - MIN_ATTACK_FREQ);
        if (HasConditionEffect(ConditionEffectIndex.Berserk))
            freq *= 1.5f;
        return freq;
    }
}
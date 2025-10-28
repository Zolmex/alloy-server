#region

using Common;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.DamageSources
{
    public abstract class DamageSource
    {
        public (ConditionEffectIndex, int)[] Effects;

        public readonly DamageBonus Bonus = new();
        public readonly HashSet<int> Hit = new();
        public abstract void SetDamage(int dmg);
        public abstract int GetDamage();
        public abstract int GetTotalDamage();
    }

    public class IndirectDamage : DamageSource
    {
        public int Damage;

        public IndirectDamage(int damage)
        {
            Damage = damage;
        }

        public override int GetDamage()
        {
            return Damage;
        }

        public override void SetDamage(int dmg)
        {
            Damage = dmg;
        }

        public override int GetTotalDamage()
        {
            return (int)((Damage * Bonus.ProportionalBonus) + Bonus.FlatBonus);
        }
    }

    public class DamageBonus
    {
        public float FlatBonus { get; set; }
        public float ProportionalBonus { get; set; } = 1f;
    }
}
#region

using Common.Resources.Config;
using System;

#endregion

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        public int TPS = GameServerConfig.Config.TPS;
        private float _hpRegenCounter;
        private float _mpRegenCounter;
        private float _msRegenCounter;

        public void TickRegens()
        {
            if (HP == MaxHP)
                _hpRegenCounter = 0;
            else
            {
                _hpRegenCounter += (float)LifeRegeneration / TPS;

                var regen = (int)_hpRegenCounter;
                if (regen > 0)
                {
                    HP = Math.Min(MaxHP, HP + regen);
                    _hpRegenCounter -= regen;
                }
            }

            if (MP == MaxMP)
                _mpRegenCounter = 0;
            else
            {
                _mpRegenCounter += (float)ManaRegeneration / TPS;

                var regen = (int)_mpRegenCounter;
                if (regen > 0)
                {
                    MP = Math.Min(MaxMP, MP + regen);
                    _mpRegenCounter -= regen;
                }
            }

            if (MS == MaxMS)
                _msRegenCounter = 0;
            else if (TimeSinceLastHit > 2000)
            {
                _msRegenCounter += (float)MaxMS / 3 * (1 + ((float)MSRegenRate / 100)) / TPS; //base 33% per second, increased by MSRegenRate, if MSRegenRate is 50, it will increase by 50%, turning 33% into 49.5%
                var regen = (int)_msRegenCounter;
                if (regen > 0)
                {
                    MS = Math.Min(MaxMS, MS + regen);
                    _msRegenCounter -= regen;
                }
            }
        }

        public void Heal(int amt, bool showText = true)
        {
            OnHeal?.Invoke(amt);
            var showAmt = HP + amt > MaxHP ? MaxHP - HP : amt;
            HP += amt;

            if (showText && showAmt != 0) SendNotif("+" + showAmt, 0x00FF00);
        }

        public void HealMP(int amt, bool showText = true)
        {
            var showAmt = MP + amt > MaxMP ? MaxMP - MP : amt;
            MP += amt;

            if (showText && showAmt != 0) SendNotif("+" + showAmt, 0x335fff);
        }

        private float GetDamageMultiplier()
        {
            //var attMult = MIN_ATTACK_MULT + Stats.Get<int>(StatType.Attack) / 75f * (MAX_ATTACK_MULT - MIN_ATTACK_MULT);

            var dmgMult = 1f + (DamageMultiplier / 100f);

            return dmgMult;
        }

        private float GetCritMultiplier(bool normalRandom = true, bool forceCrit = false)
        {
            var critMult = 1f;

            if (forceCrit || (normalRandom ? User.Random.NextIntRange(0, 1001) : User.ServerRandom.NextIntRange(0, 1001)) < CriticalChance * 10)
            {
                critMult += CriticalDamage / 100f;
            }

            return critMult;
        }

        public bool CheckDodge()
        {
            return User.Random.NextIntRange(0, 1001) < DodgeChance * 10;
        }
    }
}
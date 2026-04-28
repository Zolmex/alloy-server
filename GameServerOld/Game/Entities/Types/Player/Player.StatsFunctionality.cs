#region

using System;
using Common;
using Common.Resources.Config;

#endregion

namespace GameServerOld.Game.Entities.Types;

public partial class Player {
    private float _hpRegenCounter;
    private float _mpRegenCounter;
    private float _msRegenCounter;
    public int TPS = GameServerConfig.Config.TPS;

    public void TickRegens() {
        if (HP == MaxHP) {
            _hpRegenCounter = 0;
        }
        else {
            _hpRegenCounter += (float)Vitality / TPS;

            var regen = (int)_hpRegenCounter;
            if (regen > 0) {
                HP = Math.Min(MaxHP, HP + regen);
                _hpRegenCounter -= regen;
            }
        }

        if (MP == MaxMP) {
            _mpRegenCounter = 0;
        }
        else {
            _mpRegenCounter += (float)Wisdom / TPS;

            var regen = (int)_mpRegenCounter;
            if (regen > 0) {
                MP = Math.Min(MaxMP, MP + regen);
                _mpRegenCounter -= regen;
            }
        }
    }

    public void Heal(int amt, bool showText = true) {
        OnHeal?.Invoke(amt);
        var showAmt = HP + amt > MaxHP ? MaxHP - HP : amt;
        HP += amt;

        if (showText && showAmt != 0) SendNotif("+" + showAmt, 0x00FF00);
    }

    public void HealMP(int amt, bool showText = true) {
        var showAmt = MP + amt > MaxMP ? MaxMP - MP : amt;
        MP += amt;

        if (showText && showAmt != 0) SendNotif("+" + showAmt, 0x335fff);
    }

    private float GetDamageMultiplier() {
        if (HasConditionEffect(ConditionEffectIndex.Weak))
            return MIN_ATTACK_MULT;

        var attMult = MIN_ATTACK_MULT + Attack / 75f * (MAX_ATTACK_MULT - MIN_ATTACK_MULT);
        if (HasConditionEffect(ConditionEffectIndex.Damaging))
            attMult *= 1.5f;

        return attMult;
    }

    private float GetCritMultiplier(bool normalRandom = true, bool forceCrit = false) // TODO: remove
    {
        var critMult = 1f;

        return critMult;
    }
}
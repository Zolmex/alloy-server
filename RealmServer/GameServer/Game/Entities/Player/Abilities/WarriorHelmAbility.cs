using Common;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Diagnostics.Metrics;

namespace GameServer.Game.Entities;

public class WarriorHelmAbility : Ability
{
    private static readonly Logger _log = new Logger(typeof(WarriorHelmAbility));

    private long _cooldownReset;
    private bool _hold;
    private long _drainCooldown;
    private int _stacks;
    private long _helmDuration;
    private GemstoneBoost[] _statsModifierSave;

    public WarriorHelmAbility(Player player) : base(player)
    {
        _player.OnDamageDealt += OnDamageDealt;
        _player.OnDamagedBy += OnDamagedBy;
        _player.OnInvChanged += OnInvChanged;
        _statsModifierSave = _item.Helm.StatsModifier;
    }

    public override void Use(Item item, WorldPosData usePos, int clientTime) // Ability use is validated before this is called
    {
        _cooldownReset = RealmManager.WorldTime.TotalElapsedMs + RealmManager.TicksFromTime(item.Cooldown); // Reset cooldown
        _player.MP -= _item.MpCost;
        _hold = !_hold;
        _player.OnTick -= OnTick;
        _player.OnTick += OnTick;
        if (_hold)
            return;

        _stacks = 0;
        _player.AbilityDataA = _stacks;
        _helmDuration = RealmManager.WorldTime.TotalElapsedMs + _item.Helm.Duration;
        RealmManager.AddTimedAction(_item.Helm.Duration + 100, () => _player.OnTick -= OnTick);
    }

    public override bool Validate(Item item, Entity en)
    {
        if (item.Helm == null || _player.MP < _item.Helm.MpCost)
            return false;

        return en == _player && _cooldownReset <= RealmManager.WorldTime.TotalElapsedMs;
    }

    private void OnTick(RealmTime realmTime)
    {
        if (_hold)
            TryApllyEffects(realmTime);

        if (realmTime.TotalElapsedMs >= _helmDuration)
        {
            if (_player.AbilityDataB != "0")
                _player.AbilityDataB = 0;
            return;
        }

        _player.AbilityDataB = _helmDuration - realmTime.TotalElapsedMs;
    }

    private void TryApllyEffects(RealmTime realmTime)
    {
        foreach (var effect in _item.Helm.HoldEffects)
        {
            if (_player.HasConditionEffect(effect))
                continue;

            _player.ApplyConditionEffect(effect, 1000 / RealmManager.TPS);
        }

        if (realmTime.TotalElapsedMs < _drainCooldown)
            return;
        
        if (_player.MP > 0)
            return;

        _player.MP -= _item.Helm.MpDrain;
        _drainCooldown = realmTime.TotalElapsedMs + 1000;

        _player.MP = 0;
        _hold = false;
    }

    private void OnInvChanged(int slot, Item item)
    {
        if (slot != 1 || _item == item)
            return;
        
        foreach (var modifier in _statsModifierSave)
        {
            var stat = Enum.Parse<StatType>(modifier.Stat);
            switch (modifier.BoostType)
            {
                case "Static":
                    _player.RemoveFlatBonus(stat, modifier.Amount * _stacks);
                    break;
                case "Percentage":
                    _player.RemoveIncreasedBonus(stat, modifier.Amount * _stacks);
                    break;
            }
        }

        _item = item; // Can be null
        _player.AbilityDataA = 0;
        _stacks = 0;
        _statsModifierSave = _item == null ? new GemstoneBoost[0] : _item.Helm.StatsModifier;
    }

    private void OnDamageDealt(Character target, int damage)
    {
        if (RealmManager.WorldTime.TotalElapsedMs > _helmDuration)
            return;

        _stacks += _item.Helm.StackGain;
        _player.AbilityDataA = _stacks;

        foreach (var modifier in _item.Helm.StatsModifier)
        {
            var stat = Enum.Parse<StatType>(modifier.Stat);
            switch (modifier.BoostType)
            {
                case "Static":
                    _player.AddFlatBonus(stat, modifier.Amount * _item.Helm.StackGain);
                    break;
                case "Percentage":
                    _player.AddIncreasedBonus(stat, modifier.Amount * _item.Helm.StackGain);
                    break;
            }
        }
    }

    private void OnDamagedBy(Character arg1, Character arg2, int arg3)
    {
        if (RealmManager.WorldTime.TotalElapsedMs > _helmDuration)
            return;

        _stacks -= _item.Helm.StackLost;
        _player.AbilityDataA = _stacks;

        foreach (var modifier in _item.Helm.StatsModifier)
        {
            var stat = Enum.Parse<StatType>(modifier.Stat);
            switch (modifier.BoostType)
            {
                case "Static":
                    _player.RemoveFlatBonus(stat, modifier.Amount * _item.Helm.StackLost);
                    break;
                case "Percentage":
                    _player.RemoveIncreasedBonus(stat, modifier.Amount * _item.Helm.StackLost);
                    break;
            }
        }
    }
}
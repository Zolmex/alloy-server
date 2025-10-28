using Common;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Diagnostics.Metrics;
using System.Numerics;

namespace GameServer.Game.Entities;

public class RogueCloakAbility : Ability
{
    private static readonly Logger _log = new Logger(typeof(RogueCloakAbility));

    private long _cooldownReset;
    private bool _invisible;
    private long _useTime;

    public RogueCloakAbility(Player player) : base(player)
    {
        _player.OnInvChanged += OnInvChanged;
    }

    public override void Use(Item item, WorldPosData usePos, int clientTime) // Ability use is validated before this is called
    {
        _cooldownReset = RealmManager.WorldTime.TotalElapsedMs + RealmManager.TicksFromTime(item.Cooldown); // Reset cooldown
        _invisible = true;
        _player.OnDoShoot += OnDoShoot;
        _useTime = RealmManager.WorldTime.TotalElapsedMs;
        _player.ApplyConditionEffect(ConditionEffectIndex.Invisible, _item.Cloak.Duration);
        RealmManager.AddTimedAction(_item.Cloak.Duration, InvisibilityEnd);
    }

    public override bool Validate(Item item, Entity en)
    {
        if (item.Cloak == null || _player.MP < _item.Cloak.MpCost)
            return false;

        return en == _player && _cooldownReset <= RealmManager.WorldTime.TotalElapsedMs;
    }

    private void InvisibilityEnd()
    {
        if (_invisible == false)
            return;
        
        if (_player.HasConditionEffect(ConditionEffectIndex.Invisible))
            _player.RemoveConditionEffect(ConditionEffectIndex.Invisible);
            
        _invisible = false;
        float efficiency = (float)(RealmManager.WorldTime.TotalElapsedMs - _useTime) / (float)_item.Cloak.Duration;
        efficiency = efficiency < _item.Cloak.MinStatEfficiency ? _item.Cloak.MinStatEfficiency : efficiency;
        
        foreach (var modifier in _item.Cloak.StatsModifier)
        {
            var stat = Enum.Parse<StatType>(modifier.Stat);
            switch (modifier.BoostType)
            {
                case "Static":
                    _player.AddFlatBonus(stat, modifier.Amount * efficiency);
                    break;
                case "Percentage":
                    _player.AddIncreasedBonus(stat, modifier.Amount * efficiency);
                    break;
            }
        }
        _player.SendNotif("Efficiency: " + Math.Truncate(efficiency * 100) + "%");
        RealmManager.AddTimedAction(_item.Cloak.BoostDuration, () => RemoveBoosts(_item.Cloak.StatsModifier, efficiency));
    }

    private void RemoveBoosts(GemstoneBoost[] cloakStatsModifier, float efficiency)
    {
        foreach (var modifier in cloakStatsModifier)
        {
            var stat = Enum.Parse<StatType>(modifier.Stat);
            switch (modifier.BoostType)
            {
                case "Static":
                    _player.RemoveFlatBonus(stat, modifier.Amount * efficiency);
                    break;
                case "Percentage":
                    _player.RemoveIncreasedBonus(stat, modifier.Amount * efficiency);
                    break;
            }
        }
    }

    private void OnDoShoot(ProjectileDesc arg1, float arg2, Vector2 arg3)
    {
        _player.OnDoShoot -= OnDoShoot;
        InvisibilityEnd();
    }

    private void OnInvChanged(int slot, Item item)
    {
        if (slot != 1 || _item == item)
            return;

        if (_player.HasConditionEffect(ConditionEffectIndex.Invisible) && _invisible == true)
            _player.RemoveConditionEffect(ConditionEffectIndex.Invisible);
        
        _invisible = false;
        _item = item; // Can be null
        _player.AbilityDataA = 0;
    }
}
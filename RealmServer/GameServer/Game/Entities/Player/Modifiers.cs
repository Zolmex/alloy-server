#region

using Common;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.DamageSources;
using GameServer.Game.DamageSources.Projectiles;
using GameServer.Game.Entities.Stacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

#endregion

namespace GameServer.Game.Entities;

public enum ModTypes
{
    None,
    EquipmentBonuses
}

public abstract class Modifier
{
    protected Player _player;
    public ModTypes Id { get; set; }
    public int Order { get; set; } = -1;

    public virtual void Initialize(Player player)
    {
        _player = player;
    }

    public abstract void Apply();
    public abstract void SubscribeEvents();
    public abstract void Remove();
}

public class EquipmentBonuses : Modifier
{
    private readonly Dictionary<StatType, int> _flatBonuses = new();
    private readonly Dictionary<StatType, float> _percentBonuses = new();

    public EquipmentBonuses()
    {
        Id = ModTypes.EquipmentBonuses;
    }

    public override void Apply()
    {
        RemoveBonuses();
        for (var i = 0; i < 4; i++)
            ApplyBonuses(i);
        _player.RecalculateStats();
    }

    public override void SubscribeEvents()
    {
        _player.OnInvChanged += OnInvChanged;
    }

    public override void Remove()
    {
        _player.OnInvChanged -= OnInvChanged;
        RemoveBonuses();
    }

    private void OnInvChanged(int slot, Item item)
    {
        if (slot > 3) // Ignore changes outside of equipment
            return;

        RemoveBonuses();
        for (var i = 0; i < 4; i++)
            ApplyBonuses(i);
        _player.RecalculateStats();
    }

    private void RemoveBonuses()
    {
        foreach (var kvp in _flatBonuses)
            _player.RemoveFlatBonus(kvp.Key, kvp.Value);

        foreach (var kvp in _percentBonuses)
            _player.RemoveIncreasedBonus(kvp.Key, kvp.Value);

        _flatBonuses.Clear();
        _percentBonuses.Clear();
    }

    private void ApplyBonuses(int slot)
    {
        var item = _player.Inventory.GetItem(slot);
        ApplyEquipmentBoosts(item);
        ApplyGemstonesBoosts(item);
    }

    private void ApplyEquipmentBoosts(Item item)
    {
        if (item?.StatBoosts == null || item.StatBoosts.Length < 1)
            return;

        foreach (var boost in item.StatBoosts)
        {
            var stat = GetBoostStatType(boost.Stat);
            var amount = boost.Amount;

            if (!_flatBonuses.TryGetValue(stat, out var prevValue))
                _flatBonuses[stat] = 0;
            _flatBonuses[stat] = prevValue + amount;
            _player.AddFlatBonus(stat, amount);
        }
    }

    private void ApplyGemstonesBoosts(Item item)
    {
        if (item?.Gemstones == null || item.Gemstones.Length < 1)
            return;

        foreach (var gemType in item.Gemstones)
        {
            var gem = XmlLibrary.Gemstones[(ushort)gemType];
            if (gem.Gemstone == null || gem.Gemstone.Boosts == null)
                continue;

            foreach (var boost in gem.Gemstone.Boosts)
            {
                var amount = boost.Amount;
                if (boost.BoostTarget == "Player")
                {
                    var stat = Enum.Parse<StatType>(boost.Stat);
                    if (boost.BoostType != "Static") // Percentage boost
                    {
                        var floatAmount = amount / 100f;
                        if (!_percentBonuses.TryGetValue(stat, out var prevValue))
                            _percentBonuses[stat] = 0;
                        _percentBonuses[stat] = prevValue + floatAmount;
                        _player.AddIncreasedBonus(stat, floatAmount);
                    }
                    else
                    {
                        if (!_flatBonuses.TryGetValue(stat, out var prevValue))
                            _flatBonuses[stat] = 0;
                        _flatBonuses[stat] = (int)(prevValue + amount);
                        _player.AddFlatBonus(stat, amount);
                    }
                }
            }
        }
    }

    private static StatType GetBoostStatType(int statKey)
    {
        return statKey switch
        {
            0 => StatType.MaxHP,
            1 => StatType.MaxMP,
            2 => StatType.Attack,
            20 => StatType.Attack, // Backwards compatibility
            3 => StatType.Defense,
            21 => StatType.Defense,
            4 => StatType.Speed,
            22 => StatType.Speed,
            5 => StatType.Dexterity,
            28 => StatType.Dexterity,
            6 => StatType.Vitality,
            26 => StatType.Vitality,
            7 => StatType.Wisdom,
            27 => StatType.Wisdom,
            8 => StatType.Armor,
            29 => StatType.Armor,
            9 => StatType.CriticalDamage,
            30 => StatType.CriticalDamage,
            _ => throw new ArgumentException($"{statKey} is not a valid player stat.")
        };
    }
}

public static class ModifierRegistry
{
    private static readonly Dictionary<ModTypes, Type> _allModifiers = new();

    static ModifierRegistry()
    {
        var modifierTypes = Assembly.GetAssembly(typeof(Modifier)).GetTypes().Where(t => t.IsSubclassOf(typeof(Modifier)) && !t.IsAbstract);

        foreach (var type in modifierTypes)
        {
            var modifierInstance = (Modifier)Activator.CreateInstance(type);
            _allModifiers[modifierInstance.Id] = type;
        }
    }

    public static Modifier Get(ModTypes id)
    {
        if (_allModifiers.TryGetValue(id, out var type))
            return (Modifier)Activator.CreateInstance(type);

        return null;
    }
}
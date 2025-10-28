using Common.Utilities;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class PlayerDesc : ObjectDesc
{
    public readonly int[] SlotTypes;
    public readonly int[] Equipment;
    public Dictionary<StatType, float> Stats;

    public PlayerDesc(XElement e, string id, ushort type)
        : base(e, id, type)
    {
        SlotTypes = e.GetValue<string>("SlotTypes")?.CommaToArray<int>();
        Equipment = e.GetValue<string>("Equipment")?.CommaToArray<int>();

        Stats = new Dictionary<StatType, float>()
        {
            { StatType.MaxHP, GetDefaultStatValue(StatType.MaxHP) },
            { StatType.MaxMP, GetDefaultStatValue(StatType.MaxMP) },
            { StatType.MaxMS, GetDefaultStatValue(StatType.MaxMS) },
            { StatType.Attack, GetDefaultStatValue(StatType.Attack) },
            { StatType.Defense, GetDefaultStatValue(StatType.Defense) },
            { StatType.Dexterity, GetDefaultStatValue(StatType.Dexterity) },
            { StatType.Wisdom, GetDefaultStatValue(StatType.Wisdom) },
            { StatType.MovementSpeed, GetDefaultStatValue(StatType.MovementSpeed) },
            { StatType.LifeRegeneration, GetDefaultStatValue(StatType.LifeRegeneration) },
            { StatType.DodgeChance, GetDefaultStatValue(StatType.DodgeChance) },
            { StatType.CriticalChance, GetDefaultStatValue(StatType.CriticalChance) },
            { StatType.CriticalDamage, GetDefaultStatValue(StatType.CriticalDamage) },
            { StatType.ManaRegeneration, GetDefaultStatValue(StatType.ManaRegeneration) },
            { StatType.MSRegenRate, GetDefaultStatValue(StatType.MSRegenRate) },
            { StatType.DamageMultiplier, GetDefaultStatValue(StatType.DamageMultiplier) },
            { StatType.Armor, GetDefaultStatValue(StatType.Armor) },
            { StatType.AttackSpeed, GetDefaultStatValue(StatType.AttackSpeed) }
        };
    }

    public static float GetDefaultStatValue(StatType type)
    {
        return type switch
        {
            StatType.MaxHP => 220,
            StatType.MaxMP => 60,
            StatType.MaxMS => 10,
            StatType.Attack => 10,
            StatType.Defense => 10,
            StatType.Dexterity => 10,
            StatType.Wisdom => 10,
            StatType.MovementSpeed => 7.5f,
            StatType.LifeRegeneration => 6f,
            StatType.DodgeChance => 0f,
            StatType.CriticalChance => 0f,
            StatType.CriticalDamage => 0f,
            StatType.ManaRegeneration => 4f,
            StatType.MSRegenRate => 0f,
            StatType.DamageMultiplier => 0f,
            StatType.Armor => 0f,
            StatType.AttackSpeed => 3f,
            _ => 0
        };
    }

    public enum PlayerStatType
    {
        // Secondary stats
        MaxHP = StatType.MaxHP,
        MaxMP = StatType.MaxMP,
        MaxMS = StatType.MaxMS,
        Attack = StatType.Attack,
        Defense = StatType.Defense,
        Dexterity = StatType.Dexterity,
        Wisdom = StatType.Wisdom,
        MovementSpeed = StatType.MovementSpeed,
        LifeRegeneration = StatType.LifeRegeneration,
        DodgeChance = StatType.DodgeChance,
        CriticalChance = StatType.CriticalChance,
        CriticalDamage = StatType.CriticalDamage,
        ManaRegeneration = StatType.ManaRegeneration,
        MSRegenRate = StatType.MSRegenRate,
        DamageMultiplier = StatType.DamageMultiplier,
        Armor = StatType.Armor,
        AttackSpeed = StatType.AttackSpeed
    }
}
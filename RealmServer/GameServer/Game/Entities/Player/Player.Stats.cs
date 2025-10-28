#region

using Common;
using Common.Resources.Xml.Descriptors;
using System;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        #region STATS

        public int Fame { get => Stats.Get<int>(StatType.Fame); set => Stats.Set(StatType.Fame, value); }
        public int Gold { get => Stats.Get<int>(StatType.Credits); set => Stats.Set(StatType.Credits, value); }
        public string GuildName { get => Stats.Get<string>(StatType.GuildName); set => Stats.Set(StatType.GuildName, value); }
        public int GuildRank { get => Stats.Get<int>(StatType.GuildRank); set => Stats.Set(StatType.GuildRank, value); }
        public int Skin { get => Stats.Get<int>(StatType.Texture); set => Stats.Set(StatType.Texture, value); }

        public int MaxMP { get => Stats.Get<int>(StatType.MaxMP); set => Stats.Set(StatType.MaxMP, value); }
        public int MP { get => Stats.Get<int>(StatType.MP); set => Stats.Set(StatType.MP, value); }

        public int Attack { get => Stats.Get<int>(StatType.Attack); set => Stats.Set(StatType.Attack, value, true); }
        public int Defense { get => Stats.Get<int>(StatType.Defense); set => Stats.Set(StatType.Defense, value, true); }
        public int Dexterity { get => Stats.Get<int>(StatType.Dexterity); set => Stats.Set(StatType.Dexterity, value, true); }
        public int Wisdom { get => Stats.Get<int>(StatType.Wisdom); set => Stats.Set(StatType.Wisdom, value, true); }

        public float MovementSpeed { get => Stats.Get<float>(StatType.MovementSpeed); set => Stats.Set(StatType.MovementSpeed, value, true); }
        public int LifeRegeneration { get => Stats.Get<int>(StatType.LifeRegeneration); set => Stats.Set(StatType.LifeRegeneration, value, true); }
        public float DodgeChance { get => Stats.Get<float>(StatType.DodgeChance); set => Stats.Set(StatType.DodgeChance, value, true); }
        public float CriticalChance { get => Stats.Get<float>(StatType.CriticalChance); set => Stats.Set(StatType.CriticalChance, value, true); }
        public int CriticalDamage { get => Stats.Get<int>(StatType.CriticalDamage); set => Stats.Set(StatType.CriticalDamage, value, true); }
        public int ManaRegeneration { get => Stats.Get<int>(StatType.ManaRegeneration); set => Stats.Set(StatType.ManaRegeneration, value, true); }
        public int MSRegenRate { get => Stats.Get<int>(StatType.MSRegenRate); set => Stats.Set(StatType.MSRegenRate, value, true); }
        public int DamageMultiplier { get => Stats.Get<int>(StatType.DamageMultiplier); set => Stats.Set(StatType.DamageMultiplier, value, true); }
        public int Armor { get => Stats.Get<int>(StatType.Armor); set => Stats.Set(StatType.Armor, value, true); }
        public float AttackSpeed { get => Stats.Get<float>(StatType.AttackSpeed); set => Stats.Set(StatType.AttackSpeed, value, true); }

        public int MaxHPBonus { get => Stats.Get<int>(StatType.MaxHPBonus); set => Stats.Set(StatType.MaxHPBonus, value); }
        public int MaxMPBonus { get => Stats.Get<int>(StatType.MaxMPBonus); set => Stats.Set(StatType.MaxMPBonus, value); }
        public int MaxMSBonus { get => Stats.Get<int>(StatType.MaxMSBonus); set => Stats.Set(StatType.MaxMSBonus, value); }

        public int AttackBonus { get => Stats.Get<int>(StatType.AttackBonus); set => Stats.Set(StatType.AttackBonus, value, true); }
        public int DefenseBonus { get => Stats.Get<int>(StatType.DefenseBonus); set => Stats.Set(StatType.DefenseBonus, value, true); }
        public int DexterityBonus { get => Stats.Get<int>(StatType.DexterityBonus); set => Stats.Set(StatType.DexterityBonus, value, true); }
        public int WisdomBonus { get => Stats.Get<int>(StatType.WisdomBonus); set => Stats.Set(StatType.WisdomBonus, value, true); }

        public float MovementSpeedBonus { get => Stats.Get<float>(StatType.MovementSpeedBonus); set => Stats.Set(StatType.MovementSpeedBonus, value, true); }
        public int LifeRegenerationBonus { get => Stats.Get<int>(StatType.LifeRegenerationBonus); set => Stats.Set(StatType.LifeRegenerationBonus, value, true); }
        public float DodgeChanceBonus { get => Stats.Get<float>(StatType.DodgeChanceBonus); set => Stats.Set(StatType.DodgeChanceBonus, value, true); }
        public float CriticalChanceBonus { get => Stats.Get<float>(StatType.CriticalChanceBonus); set => Stats.Set(StatType.CriticalChanceBonus, value, true); }
        public int CriticalDamageBonus { get => Stats.Get<int>(StatType.CriticalDamageBonus); set => Stats.Set(StatType.CriticalDamageBonus, value, true); }
        public int ManaRegenerationBonus { get => Stats.Get<int>(StatType.ManaRegenerationBonus); set => Stats.Set(StatType.ManaRegenerationBonus, value, true); }
        public int MSRegenRateBonus { get => Stats.Get<int>(StatType.MSRegenRateBonus); set => Stats.Set(StatType.MSRegenRateBonus, value, true); }
        public int DamageBonus { get => Stats.Get<int>(StatType.DamageBonus); set => Stats.Set(StatType.DamageBonus, value, true); }
        public int ArmorBonus { get => Stats.Get<int>(StatType.ArmorBonus); set => Stats.Set(StatType.ArmorBonus, value, true); }
        public float AttackSpeedBonus { get => Stats.Get<float>(StatType.AttackSpeedBonus); set => Stats.Set(StatType.AttackSpeedBonus, value, true); }

        public int AccRank { get => Stats.Get<int>(StatType.AccRank); set => Stats.Set(StatType.AccRank, value); }
        public int PartyId { get => Stats.Get<int>(StatType.PartyId); set => Stats.Set(StatType.PartyId, value); }
        public int HealthPotions { get => Stats.Get<int>(StatType.HealthPotionStack); set => Stats.Set(StatType.HealthPotionStack, value); }
        public int MagicPotions { get => Stats.Get<int>(StatType.MagicPotionStack); set => Stats.Set(StatType.MagicPotionStack, value); }
        
        // AbilityData is serialized as string
        public object AbilityDataA { get => Stats.Get<string>(StatType.AbilityDataA); set => Stats.Set(StatType.AbilityDataA, value.ToString()); }
        public object AbilityDataB { get => Stats.Get<string>(StatType.AbilityDataB); set => Stats.Set(StatType.AbilityDataB, value.ToString()); }
        public object AbilityDataC { get => Stats.Get<string>(StatType.AbilityDataC); set => Stats.Set(StatType.AbilityDataC, value.ToString()); }
        public object AbilityDataD { get => Stats.Get<string>(StatType.AbilityDataD); set => Stats.Set(StatType.AbilityDataD, value.ToString()); }

        #endregion

        private readonly Dictionary<StatType, StatBonus> _statBonuses = new();

        public void InitStatBonuses()
        {
            foreach (var kvp in Char.SecondaryStats)
            {
                _statBonuses.TryAdd(kvp.Key, new StatBonus());
            }
        }

        private void ModifyBonus(StatType statType, Action<StatBonus> modifyAction)
        {
            var bonus = _statBonuses[statType];
            modifyAction(bonus);
            RecalculateStat(statType);
        }

        public void RecalculateStats()
        {
            // Update base values first
            RecalculateStat(StatType.Attack);
            RecalculateStat(StatType.Defense);
            RecalculateStat(StatType.Dexterity);
            RecalculateStat(StatType.Wisdom);

            // Recalculate secondary stats
            foreach (var statType in Enum.GetValues(typeof(PlayerDesc.PlayerStatType))) //Convert this call Enum.GetValues() to a static array? - Evil
                RecalculateStat((StatType)statType);
        }

        public void RecalculateStat(StatType statType)
        {
            var baseStatType = GetBaseStatType(statType);
            var statPoints = 0;
            if (baseStatType != StatType.None)
            {
                statPoints = Char.MainStats[baseStatType]; // Need to calculate the baseValue based on stat points
                if (baseStatType != statType) // Att, def, dex, wis shouldn't recalculate with their boosts, but the other stats should consider the total of the base stat
                    statPoints = Stats.Get<int>(baseStatType);
            }

            // Not all stats depend on a base stat
            var baseValue = baseStatType == StatType.None ? Char.BaseStats[statType] : CalculateBaseValue(statType, statPoints);
            var bonus = _statBonuses[statType];

            // I need increased/reduced and more/less to be explained to me in order to understand this
            var increasedReducedMultiplier = 1f + (bonus.IncreasedBonus / 100) - (bonus.ReducedBonus / 100);
            var moreLessMultiplier = (1f + (bonus.MoreBonus / 100)) * (1f - (bonus.LessBonus / 100));
            var finalValue = (baseValue + bonus.FlatBonus) * increasedReducedMultiplier * moreLessMultiplier;
            var bonusValue = finalValue - baseValue;

            finalValue = Round(finalValue);

            var bonusStat = GetBonusStat(statType);
            var isPrivate = IsPrivate(statType);
            if (StatData.IsFloatStat(statType)) // Breaks if it's done any other way...
            {
                Stats.Set(statType, finalValue, isPrivate);
                if (bonusStat != StatType.None)
                    Stats.Set(bonusStat, finalValue, isPrivate);
            }
            else
            {
                var intValue = Convert.ToInt32(finalValue);
                var intBonusValue = Convert.ToInt32(bonusValue);
                Stats.Set(statType, intValue, isPrivate);
                if (bonusStat != StatType.None)
                    Stats.Set(bonusStat, intBonusValue, isPrivate);
            }
        }

        private float Round(float value)
        {
            if (value % 1 != 0)
                return (float)Math.Round(value, 1, MidpointRounding.AwayFromZero);

            return value;
        }

        private void ResetBonuses()
        {
            foreach (var kvp in _statBonuses)
            {
                kvp.Value.FlatBonus = 0;
                kvp.Value.ReducedBonus = 0;
                kvp.Value.IncreasedBonus = 0;
                kvp.Value.LessBonus = 0;
                kvp.Value.MoreBonus = 0;
                RecalculateStat(kvp.Key);
            }
        }

        public void AddFlatBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.FlatBonus += Round(value));
        }

        public void RemoveFlatBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.FlatBonus -= Round(value));
        }

        public void SetFlatBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.FlatBonus = Round(value));
        }

        public void AddIncreasedBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.IncreasedBonus += Round(value));
        }

        public void RemoveIncreasedBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.IncreasedBonus -= Round(value));
        }

        public void SetIncreasedBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.IncreasedBonus = Round(value));
        }

        public void AddReducedBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.ReducedBonus += Round(value));
        }

        public void RemoveReducedBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.ReducedBonus -= Round(value));
        }

        public void SetReducedBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.ReducedBonus = Round(value));
        }

        public void AddMoreBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.MoreBonus += Round(value));
        }

        public void RemoveMoreBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.MoreBonus -= Round(value));
        }

        public void SetMoreBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.MoreBonus = Round(value));
        }

        public void AddLessBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.LessBonus += Round(value));
        }

        public void RemoveLessBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.LessBonus -= Round(value));
        }

        public void SetLessBonus(StatType statType, float value)
        {
            ModifyBonus(statType, b => b.LessBonus = Round(value));
        }

        private static bool IsPrivate(StatType stat)
        {
            return stat switch
            {
                StatType.MaxHP => false,
                StatType.MaxMP => false,
                StatType.MaxMS => false,
                _ => true
            };
        }

        private static StatType GetBonusStat(StatType stat)
        {
            return stat switch
            {
                StatType.MaxHP => StatType.MaxHPBonus,
                StatType.MaxMP => StatType.MaxMPBonus,
                StatType.MaxMS => StatType.MaxMSBonus,
                StatType.Attack => StatType.AttackBonus,
                StatType.Defense => StatType.DefenseBonus,
                StatType.MovementSpeed => StatType.MovementSpeedBonus,
                StatType.Dexterity => StatType.DexterityBonus,
                StatType.LifeRegeneration => StatType.LifeRegenerationBonus,
                StatType.Wisdom => StatType.WisdomBonus,
                StatType.DodgeChance => StatType.DodgeChanceBonus,
                StatType.CriticalChance => StatType.CriticalChanceBonus,
                StatType.CriticalDamage => StatType.CriticalDamageBonus,
                StatType.ManaRegeneration => StatType.ManaRegenerationBonus,
                StatType.MSRegenRate => StatType.MSRegenRateBonus,
                StatType.Armor => StatType.ArmorBonus,
                StatType.DamageMultiplier => StatType.DamageBonus,
                StatType.AttackSpeed => StatType.AttackSpeedBonus,
                _ => StatType.None
            };
        }

        private static float CalculateBaseValue(StatType type, int statPoints)
        {
            return type switch
            {
                StatType.MaxHP => MaxHPCalc(statPoints),
                StatType.MaxMP => MaxMPCalc(statPoints),
                StatType.MaxMS => MaxMSCalc(statPoints),
                StatType.DamageMultiplier => DamageMultCalc(statPoints),
                StatType.CriticalDamage => CritDmgCalc(statPoints),
                StatType.Armor => ArmorCalc(statPoints),
                StatType.LifeRegeneration => LifeRegenCalc(statPoints),
                StatType.DodgeChance => DodgeCalc(statPoints),
                StatType.CriticalChance => CritChanceCalc(statPoints),
                StatType.AttackSpeed => AttackSpeedCalc(statPoints),
                StatType.ManaRegeneration => ManaRegenCalc(statPoints),
                _ => statPoints
            };
        }

        private static StatType GetBaseStatType(StatType type)
        {
            return type switch
            {
                StatType.Attack => StatType.Attack,
                StatType.DamageMultiplier => StatType.Attack,
                StatType.CriticalDamage => StatType.Attack,
                StatType.Defense => StatType.Defense,
                StatType.MaxHP => StatType.Defense,
                StatType.Armor => StatType.Defense,
                StatType.LifeRegeneration => StatType.Defense,
                StatType.Dexterity => StatType.Dexterity,
                StatType.DodgeChance => StatType.Dexterity,
                StatType.CriticalChance => StatType.Dexterity,
                StatType.AttackSpeed => StatType.Dexterity,
                StatType.Wisdom => StatType.Wisdom,
                StatType.MaxMP => StatType.Wisdom,
                StatType.MaxMS => StatType.Wisdom,
                StatType.ManaRegeneration => StatType.Wisdom,
                _ => StatType.None
            };
        }

        public static float DamageMultCalc(int statPoints)
        {
            return statPoints * 2f;
        }

        public static float CritDmgCalc(int statPoints)
        {
            return statPoints;
        }

        public static float MaxHPCalc(int statPoints)
        {
            return PlayerDesc.GetDefaultStatValue(StatType.MaxHP) + (statPoints * 8f);
        }

        public static float ArmorCalc(int statPoints)
        {
            return statPoints / 2f;
        }

        public static float DodgeCalc(int statPoints)
        {
            return statPoints / 10f;
        }

        public static float CritChanceCalc(int statPoints)
        {
            return statPoints / 10f;
        }

        public static float MaxMPCalc(int statPoints)
        {
            return PlayerDesc.GetDefaultStatValue(StatType.MaxMP) + (statPoints * 4f);
        }

        public static float MaxMSCalc(int statPoints)
        {
            return statPoints * 2;
        }


        public static float LifeRegenCalc(int statPoints)
        {
            return PlayerDesc.GetDefaultStatValue(StatType.LifeRegeneration) + (statPoints / 5f);
        }


        public static float AttackSpeedCalc(int statPoints)
        {
            return PlayerDesc.GetDefaultStatValue(StatType.AttackSpeed) + (statPoints / 20f);
        }


        public static float ManaRegenCalc(int statPoints)
        {
            return PlayerDesc.GetDefaultStatValue(StatType.ManaRegeneration) + (statPoints / 10f);
        }

        public class StatBonus
        {
            public float FlatBonus { get; set; } = 0;
            public float IncreasedBonus { get; set; } = 0;
            public float ReducedBonus { get; set; } = 0;
            public float MoreBonus { get; set; } = 0;
            public float LessBonus { get; set; } = 0;
        }
    }
}
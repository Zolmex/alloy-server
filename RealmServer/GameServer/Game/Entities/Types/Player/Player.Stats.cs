#region

using Common;
using Common.Resources.Xml.Descriptors;
using System;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities.Types;

public partial class Player
{
    #region STATS

    public int Fame { get => Stats.GetInt(StatType.Fame); set => Stats.Set(StatType.Fame, value); }
    public int Gold { get => Stats.GetInt(StatType.Credits); set => Stats.Set(StatType.Credits, value); }
    public string GuildName { get => Stats.GetString(StatType.GuildName); set => Stats.Set(StatType.GuildName, value); }
    public int GuildRank { get => Stats.GetInt(StatType.GuildRank); set => Stats.Set(StatType.GuildRank, value); }
    public int Skin { get => Stats.GetInt(StatType.Texture); set => Stats.Set(StatType.Texture, value); }

    public int MaxMP { get => Stats.GetInt(StatType.MaxMP); set => Stats.Set(StatType.MaxMP, value); }
    public int MP { get => Stats.GetInt(StatType.MP); set => Stats.Set(StatType.MP, value); }

    public int Attack { get => Stats.GetInt(StatType.Attack); set => Stats.Set(StatType.Attack, value, true); }
    public int Defense { get => Stats.GetInt(StatType.Defense); set => Stats.Set(StatType.Defense, value, true); }
    public int Speed { get => Stats.GetInt(StatType.Speed); set => Stats.Set(StatType.Speed, value, true); }
    public int Dexterity { get => Stats.GetInt(StatType.Dexterity); set => Stats.Set(StatType.Dexterity, value, true); }
    public int Vitality { get => Stats.GetInt(StatType.Vitality); set => Stats.Set(StatType.Vitality, value, true); }
    public int Wisdom { get => Stats.GetInt(StatType.Wisdom); set => Stats.Set(StatType.Wisdom, value, true); }

    public int MaxHPBonus { get => Stats.GetInt(StatType.MaxHPBonus); set => Stats.Set(StatType.MaxHPBonus, value); }
    public int MaxMPBonus { get => Stats.GetInt(StatType.MaxMPBonus); set => Stats.Set(StatType.MaxMPBonus, value); }

    public int AttackBonus { get => Stats.GetInt(StatType.AttackBonus); set => Stats.Set(StatType.AttackBonus, value, true); }
    public int DefenseBonus { get => Stats.GetInt(StatType.DefenseBonus); set => Stats.Set(StatType.DefenseBonus, value, true); }
    public float SpeedBonus { get => Stats.GetFloat(StatType.SpeedBonus); set => Stats.Set(StatType.SpeedBonus, value, true); }
    public int DexterityBonus { get => Stats.GetInt(StatType.DexterityBonus); set => Stats.Set(StatType.DexterityBonus, value, true); }
    public int VitalityBonus { get => Stats.GetInt(StatType.VitalityBonus); set => Stats.Set(StatType.VitalityBonus, value, true); }
    public int WisdomBonus { get => Stats.GetInt(StatType.WisdomBonus); set => Stats.Set(StatType.WisdomBonus, value, true); }

    public int AccRank { get => Stats.GetInt(StatType.AccRank); set => Stats.Set(StatType.AccRank, value); }
    public int HealthPotions { get => Stats.GetInt(StatType.HealthPotionStack); set => Stats.Set(StatType.HealthPotionStack, value); }
    public int MagicPotions { get => Stats.GetInt(StatType.MagicPotionStack); set => Stats.Set(StatType.MagicPotionStack, value); }

    #endregion
}
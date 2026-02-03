using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Character : IDbModel
{
    public string Key => $"character.{Id}";
    
    public int Id { get; set; }

    public int? AccCharId { get; set; }

    public ushort? ObjectType { get; set; }

    public ushort? Level { get; set; }

    public uint? CurrentFame { get; set; }

    public uint? XpPoints { get; set; }

    public ushort? SkinType { get; set; }

    public ushort? TextureOne { get; set; }

    public ushort? TextureTwo { get; set; }

    public ushort? PetType { get; set; }

    public ushort? HealthPotions { get; set; }

    public ushort? MagicPotions { get; set; }

    public bool IsDead { get; set; }

    public bool IsDeleted { get; set; }

    public bool HasBackpack { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? AccId { get; set; }

    public int? CharStatsId { get; set; }

    public int? ExploStatsId { get; set; }

    public int? CombatStatsId { get; set; }

    public int? KillStatsId { get; set; }

    public int? DungeonStatsId { get; set; }

    public virtual Account? Acc { get; set; }

    public virtual CharacterStat? CharStats { get; set; }

    public virtual ICollection<CharacterDeath> CharacterDeaths { get; set; } = new List<CharacterDeath>();

    public virtual ICollection<CharacterInventory> CharacterInventories { get; set; } = new List<CharacterInventory>();

    public virtual CombatStat? CombatStats { get; set; }

    public virtual DungeonStat? DungeonStats { get; set; }

    public virtual ExplorationStat? ExploStats { get; set; }

    public virtual KillStat? KillStats { get; set; }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(AccCharId ?? 0);
        wtr.Write(ObjectType ?? 0);
        wtr.Write(Level ?? 0);
        wtr.Write(CurrentFame ?? 0);
        wtr.Write(XpPoints ?? 0);
        wtr.Write(SkinType ?? 0);
        wtr.Write(TextureOne ?? 0);
        wtr.Write(TextureTwo ?? 0);
        wtr.Write(PetType ?? 0);
        wtr.Write(HealthPotions ?? 0);
        wtr.Write(MagicPotions ?? 0);
        wtr.Write(IsDead);
        wtr.Write(IsDeleted);
        wtr.Write(HasBackpack);
        wtr.Write(CreatedAt!.Value.ToUnixTimestamp());
        wtr.Write((DeletedAt ?? DateTime.MinValue).ToUnixTimestamp());
        wtr.Write(AccId ?? 0);
        if (CharStats != null)
            CharStats.Write(wtr);
        else wtr.Write(0);
        if (CombatStats != null)
            CombatStats.Write(wtr);
        else wtr.Write(0);
        if (DungeonStats != null)
            DungeonStats.Write(wtr);
        else wtr.Write(0);
        if (ExploStats != null)
            ExploStats.Write(wtr);
        else wtr.Write(0);
        if (KillStats != null)
            KillStats.Write(wtr);
        else wtr.Write(0);
    }

    public static Character Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;
        
        var ret = new Character();
        ret.Id = id;
        ret.AccCharId = rdr.ReadInt32();
        ret.ObjectType = rdr.ReadUInt16();
        ret.Level = rdr.ReadUInt16();
        ret.CurrentFame = rdr.ReadUInt32();
        ret.XpPoints = rdr.ReadUInt32();
        ret.SkinType = rdr.ReadUInt16();
        ret.TextureOne = rdr.ReadUInt16();
        ret.TextureTwo = rdr.ReadUInt16();
        ret.PetType = rdr.ReadUInt16();
        ret.HealthPotions = rdr.ReadUInt16();
        ret.MagicPotions = rdr.ReadUInt16();
        ret.IsDead = rdr.ReadBoolean();
        ret.IsDeleted = rdr.ReadBoolean();
        ret.HasBackpack = rdr.ReadBoolean();
        ret.CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        ret.DeletedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        ret.AccId = rdr.ReadInt32();
        ret.CharStats = CharacterStat.Read(rdr);
        ret.CharStatsId = ret.CharStats?.Id ?? 0;
        ret.ExploStats = ExplorationStat.Read(rdr);
        ret.ExploStatsId = ret.ExploStats?.Id ?? 0;
        ret.CombatStats = CombatStat.Read(rdr);
        ret.CombatStatsId = ret.CombatStats?.Id ?? 0;
        ret.KillStats = KillStat.Read(rdr);
        ret.KillStatsId = ret.KillStats?.Id ?? 0;
        ret.DungeonStats = DungeonStat.Read(rdr);
        ret.DungeonStatsId = ret.DungeonStats?.Id ?? 0;
        return ret;
    }
}

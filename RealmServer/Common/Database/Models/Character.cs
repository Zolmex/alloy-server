using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Character
{
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
        wtr.Write((byte)1);
        
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
        wtr.Write(CharStatsId ?? 0);
        wtr.Write(ExploStatsId ?? 0);
        wtr.Write(CombatStatsId ?? 0);
        wtr.Write(KillStatsId ?? 0);
        wtr.Write(DungeonStatsId ?? 0);
        
        if (CharStats != null)
            CharStats.Write(wtr);
        else wtr.Write((byte)0);
        if (CombatStats != null)
            CombatStats.Write(wtr);
        else wtr.Write((byte)0);
        if (DungeonStats != null)
            DungeonStats.Write(wtr);
        else wtr.Write((byte)0);
        if (ExploStats != null)
            ExploStats.Write(wtr);
        else wtr.Write((byte)0);
        if (KillStats != null)
            KillStats.Write(wtr);
        else wtr.Write((byte)0);
    }

    public static Character Read(NetworkReader rdr)
    {
        if (rdr.ReadByte() == 0) // Empty flag
            return null;
        
        var ret = new Character();
        ret.Id = rdr.ReadInt32();
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
        ret.CharStatsId = rdr.ReadInt32();
        ret.ExploStatsId = rdr.ReadInt32();
        ret.CombatStatsId = rdr.ReadInt32();
        ret.KillStatsId = rdr.ReadInt32();
        ret.DungeonStatsId = rdr.ReadInt32();
        ret.CharStats = CharacterStat.Read(rdr);
        ret.CombatStats = CombatStat.Read(rdr);
        ret.DungeonStats = DungeonStat.Read(rdr);
        ret.ExploStats = ExplorationStat.Read(rdr);
        ret.KillStats = KillStat.Read(rdr);
        return ret;
    }
}

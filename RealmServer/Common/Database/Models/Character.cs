using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Character : IDbSerializable
{
    public int Id { get; set; }

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

    public bool? IsDead { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? HasBackpack { get; set; }

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
        wtr.Write(ObjectType!.Value);
        wtr.Write(Level!.Value);
        wtr.Write(CurrentFame!.Value);
        wtr.Write(XpPoints!.Value);
        wtr.Write(SkinType!.Value);
        wtr.Write(TextureOne!.Value);
        wtr.Write(TextureTwo!.Value);
        wtr.Write(PetType!.Value);
        wtr.Write(HealthPotions!.Value);
        wtr.Write(MagicPotions!.Value);
        wtr.Write(IsDead!.Value);
        wtr.Write(IsDeleted!.Value);
        wtr.Write(HasBackpack!.Value);
        wtr.Write(CreatedAt!.Value.ToUnixTimestamp());
        wtr.Write(DeletedAt!.Value.ToUnixTimestamp());
        wtr.Write(AccId!.Value);
        wtr.Write(CharStatsId!.Value);
        wtr.Write(ExploStatsId!.Value);
        wtr.Write(CombatStatsId!.Value);
        wtr.Write(KillStatsId!.Value);
        wtr.Write(DungeonStatsId!.Value);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new Character()
        {
            Id = rdr.ReadInt32(),
            ObjectType = rdr.ReadUInt16(),
            Level = rdr.ReadUInt16(),
            CurrentFame = rdr.ReadUInt32(),
            XpPoints = rdr.ReadUInt32(),
            SkinType = rdr.ReadUInt16(),
            TextureOne = rdr.ReadUInt16(),
            TextureTwo = rdr.ReadUInt16(),
            PetType = rdr.ReadUInt16(),
            HealthPotions = rdr.ReadUInt16(),
            MagicPotions = rdr.ReadUInt16(),
            IsDead = rdr.ReadBoolean(),
            IsDeleted = rdr.ReadBoolean(),
            HasBackpack = rdr.ReadBoolean(),
            CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32()),
            DeletedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32()),
            AccId = rdr.ReadInt32(),
            CharStatsId = rdr.ReadInt32(),
            ExploStatsId = rdr.ReadInt32(),
            CombatStatsId = rdr.ReadInt32(),
            KillStatsId = rdr.ReadInt32(),
            DungeonStatsId = rdr.ReadInt32()
        };
    }
}

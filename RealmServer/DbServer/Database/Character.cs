using System;
using System.Collections.Generic;

namespace DbServer.Database;

public partial class Character
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
}

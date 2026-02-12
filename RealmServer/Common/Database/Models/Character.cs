using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Character : DbModel, IDbQueryable
{
    public const string KEY_BASE = "character";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public int AccCharId { get; set; }

    public ushort ObjectType { get; set; }

    public ushort Level { get; set; }

    public uint CurrentFame { get; set; }

    public uint XpPoints { get; set; }

    public ushort SkinType { get; set; }

    public ushort TextureOne { get; set; }

    public ushort TextureTwo { get; set; }

    public ushort PetType { get; set; }

    public ushort HealthPotions { get; set; }

    public ushort MagicPotions { get; set; }

    public bool IsDead { get; set; }

    public bool IsDeleted { get; set; }

    public bool HasBackpack { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int AccId { get; set; }

    public int CharStatsId { get; set; }

    public int ExploStatsId { get; set; }

    public int CombatStatsId { get; set; }

    public int KillStatsId { get; set; }

    public int DungeonStatsId { get; set; }

    public virtual Account? Acc { get; set; }

    public virtual CharacterStat? CharStats { get; set; }

    public virtual ICollection<CharacterDeath> CharacterDeaths { get; set; } = new List<CharacterDeath>();

    public virtual ICollection<CharacterInventory> CharacterInventories { get; set; } = new List<CharacterInventory>();

    public virtual CombatStat? CombatStats { get; set; }

    public virtual DungeonStat? DungeonStats { get; set; }

    public virtual ExplorationStat? ExploStats { get; set; }

    public virtual KillStat? KillStats { get; set; }

    public Character()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("AccCharId",
            wtr => wtr.Write(AccCharId),
            rdr => AccCharId = rdr.ReadInt32()
        );
        RegisterProperty("ObjectType",
            wtr => wtr.Write(ObjectType),
            rdr => ObjectType = rdr.ReadUInt16()
        );
        RegisterProperty("Level",
            wtr => wtr.Write(Level),
            rdr => Level = rdr.ReadUInt16()
        );
        RegisterProperty("CurrentFame",
            wtr => wtr.Write(CurrentFame),
            rdr => CurrentFame = rdr.ReadUInt32()
        );
        RegisterProperty("XpPoints",
            wtr => wtr.Write(XpPoints),
            rdr => XpPoints = rdr.ReadUInt32()
        );
        RegisterProperty("SkinType",
            wtr => wtr.Write(SkinType),
            rdr => SkinType = rdr.ReadUInt16()
        );
        RegisterProperty("TextureOne",
            wtr => wtr.Write(TextureOne),
            rdr => TextureOne = rdr.ReadUInt16()
        );
        RegisterProperty("TextureTwo",
            wtr => wtr.Write(TextureTwo),
            rdr => TextureTwo = rdr.ReadUInt16()
        );
        RegisterProperty("PetType",
            wtr => wtr.Write(PetType),
            rdr => PetType = rdr.ReadUInt16()
        );
        RegisterProperty("HealthPotions",
            wtr => wtr.Write(HealthPotions),
            rdr => HealthPotions = rdr.ReadUInt16()
        );
        RegisterProperty("IsDead",
            wtr => wtr.Write(IsDead),
            rdr => IsDead = rdr.ReadBoolean()
        );
        RegisterProperty("IsDeleted",
            wtr => wtr.Write(IsDeleted),
            rdr => IsDeleted = rdr.ReadBoolean()
        );
        RegisterProperty("HasBackpack",
            wtr => wtr.Write(HasBackpack),
            rdr => HasBackpack = rdr.ReadBoolean()
        );
        RegisterProperty("CreatedAt",
            wtr => wtr.Write(CreatedAt!.Value.ToUnixTimestamp()),
            rdr => CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("DeletedAt",
            wtr => wtr.Write((DeletedAt ?? DateTime.MinValue).ToUnixTimestamp()),
            rdr => DeletedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("AccId",
            wtr => wtr.Write(AccId),
            rdr => AccId = rdr.ReadInt32()
        );
        RegisterProperty("CharStats",
            wtr =>
            {
                var hasValue = CharStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    CharStats.WriteProperties(wtr);
            },
            rdr =>
            {
                CharStats = DbModel.Read<CharacterStat>(rdr);
                CharStatsId = CharStats?.Id ?? 0;
            }
        );
        RegisterProperty("CombatStats",
            wtr =>
            {
                var hasValue = CombatStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    CombatStats.WriteProperties(wtr);
            },
            rdr =>
            {
                CombatStats = DbModel.Read<CombatStat>(rdr);
                CombatStatsId = CombatStats?.Id ?? 0;
            }
        );
        RegisterProperty("DungeonStats",
            wtr =>
            {
                var hasValue = DungeonStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    DungeonStats.WriteProperties(wtr);
            },
            rdr =>
            {
                DungeonStats = DbModel.Read<DungeonStat>(rdr);
                DungeonStatsId = DungeonStats?.Id ?? 0;
            }
        );
        RegisterProperty("ExploStats",
            wtr =>
            {
                var hasValue = ExploStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    ExploStats.WriteProperties(wtr);
            },
            rdr =>
            {
                ExploStats = DbModel.Read<ExplorationStat>(rdr);
                ExploStatsId = ExploStats?.Id ?? 0;
            }
        );
        RegisterProperty("KillStats",
            wtr =>
            {
                var hasValue = KillStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    KillStats.WriteProperties(wtr);
            },
            rdr =>
            {
                KillStats = DbModel.Read<KillStat>(rdr);
                KillStatsId = KillStats?.Id ?? 0;
            }
        );
        RegisterProperty("CharacterInventories",
            wtr =>
            {
                wtr.Write((short)CharacterInventories.Count);
                foreach (var inv in CharacterInventories)
                {
                    var hasValue = inv != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        inv.WriteProperties(wtr);
                }
            },
            rdr =>
            {
                CharacterInventories.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var inv = DbModel.Read<CharacterInventory>(rdr);
                    if (inv != null)
                        CharacterInventories.Add(inv);
                }
            }
        );
    }

    public static Character Read(string key)
    {
        var ret = new Character();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }
    
    public static IEnumerable<string> GetIncludes()
    {
        yield return "CharStats";
        yield return "CombatStats";
        yield return "DungeonStats";
        yield return "ExploStats";
        yield return "KillStats";
        yield return "CharacterInventories";
    }
    
    public static string BuildKey(int id)
    {
        return KEY_BASE + $".{id}";
    }
}

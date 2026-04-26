using System;
using System.Collections.Generic;
using Common.Utilities;

namespace Common.Database.Models;

public class Character : DbModel, IDbQueryable {
    public const string KEY_BASE = "character";

    public Character() {
        RegisterProperty("Id",
            (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("AccCharId",
            (ref wtr) => wtr.Write(AccCharId),
            (ref rdr) => AccCharId = rdr.ReadInt32()
        );
        RegisterProperty("ObjectType",
            (ref wtr) => wtr.Write(ObjectType),
            (ref rdr) => ObjectType = rdr.ReadUInt16()
        );
        RegisterProperty("Level",
            (ref wtr) => wtr.Write(Level),
            (ref rdr) => Level = rdr.ReadUInt16()
        );
        RegisterProperty("CurrentFame",
            (ref wtr) => wtr.Write(CurrentFame),
            (ref rdr) => CurrentFame = rdr.ReadUInt32()
        );
        RegisterProperty("XpPoints",
            (ref wtr) => wtr.Write(XpPoints),
            (ref rdr) => XpPoints = rdr.ReadUInt32()
        );
        RegisterProperty("SkinType",
            (ref wtr) => wtr.Write(SkinType),
            (ref rdr) => SkinType = rdr.ReadUInt16()
        );
        RegisterProperty("TextureOne",
            (ref wtr) => wtr.Write(TextureOne),
            (ref rdr) => TextureOne = rdr.ReadUInt16()
        );
        RegisterProperty("TextureTwo",
            (ref wtr) => wtr.Write(TextureTwo),
            (ref rdr) => TextureTwo = rdr.ReadUInt16()
        );
        RegisterProperty("PetType",
            (ref wtr) => wtr.Write(PetType),
            (ref rdr) => PetType = rdr.ReadUInt16()
        );
        RegisterProperty("HealthPotions",
            (ref wtr) => wtr.Write(HealthPotions),
            (ref rdr) => HealthPotions = rdr.ReadUInt16()
        );
        RegisterProperty("IsDead",
            (ref wtr) => wtr.Write(IsDead),
            (ref rdr) => IsDead = rdr.ReadBoolean()
        );
        RegisterProperty("IsDeleted",
            (ref wtr) => wtr.Write(IsDeleted),
            (ref rdr) => IsDeleted = rdr.ReadBoolean()
        );
        RegisterProperty("HasBackpack",
            (ref wtr) => wtr.Write(HasBackpack),
            (ref rdr) => HasBackpack = rdr.ReadBoolean()
        );
        RegisterProperty("CreatedAt",
            (ref wtr) => wtr.Write(CreatedAt.ToUnixTimestamp()),
            (ref rdr) => CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("DeletedAt",
            (ref wtr) => wtr.Write((DeletedAt ?? DateTime.MinValue).ToUnixTimestamp()),
            (ref rdr) => DeletedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("AccId",
            (ref wtr) => wtr.Write(AccId),
            (ref rdr) => AccId = rdr.ReadInt32()
        );
        RegisterProperty("CharStats",
            (ref wtr) => {
                var hasValue = CharStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    CharStats.WriteProperties(ref wtr);
            },
            (ref rdr) => {
                CharStats = Read<CharacterStat>(ref rdr);
                CharStatsId = CharStats?.Id ?? 0;
            }
        );
        RegisterProperty("CombatStats",
            (ref wtr) => {
                var hasValue = CombatStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    CombatStats.WriteProperties(ref wtr);
            },
            (ref rdr) => {
                CombatStats = Read<CombatStat>(ref rdr);
                CombatStatsId = CombatStats?.Id ?? 0;
            }
        );
        RegisterProperty("DungeonStats",
            (ref wtr) => {
                var hasValue = DungeonStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    DungeonStats.WriteProperties(ref wtr);
            },
            (ref rdr) => {
                DungeonStats = Read<DungeonStat>(ref rdr);
                DungeonStatsId = DungeonStats?.Id ?? 0;
            }
        );
        RegisterProperty("ExploStats",
            (ref wtr) => {
                var hasValue = ExploStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    ExploStats.WriteProperties(ref wtr);
            },
            (ref rdr) => {
                ExploStats = Read<ExplorationStat>(ref rdr);
                ExploStatsId = ExploStats?.Id ?? 0;
            }
        );
        RegisterProperty("KillStats",
            (ref wtr) => {
                var hasValue = KillStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    KillStats.WriteProperties(ref wtr);
            },
            (ref rdr) => {
                KillStats = Read<KillStat>(ref rdr);
                KillStatsId = KillStats?.Id ?? 0;
            }
        );
        RegisterProperty("CharacterInventories",
            (ref wtr) => {
                wtr.Write((short)CharacterInventories.Count);
                foreach (var inv in CharacterInventories) {
                    var hasValue = inv != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        inv.WriteProperties(ref wtr);
                }
            },
            (ref rdr) => {
                CharacterInventories.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++) {
                    var inv = Read<CharacterInventory>(ref rdr);
                    if (inv != null)
                        CharacterInventories.Add(inv);
                }
            }
        );
    }

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

    public DateTime CreatedAt { get; set; }

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

    public static IEnumerable<string> GetIncludes() {
        yield return "CharStats";
        yield return "CombatStats";
        yield return "DungeonStats";
        yield return "ExploStats";
        yield return "KillStats";
        yield return "CharacterInventories";
    }

    public static Character Read(string key) {
        var ret = new Character();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }

    public static string BuildKey(int id) {
        return KEY_BASE + $".{id}";
    }
}
using System.Collections.Generic;

namespace Common.Database.Models;

public class ClassStat : DbModel, IDbQueryable {
    public const string KEY_BASE = "classStat";

    public ClassStat() {
        RegisterProperty("Id",
            (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("ObjectType",
            (ref wtr) => wtr.Write(ObjectType),
            (ref rdr) => ObjectType = rdr.ReadUInt16()
        );
        RegisterProperty("BestLevel",
            (ref wtr) => wtr.Write(BestLevel),
            (ref rdr) => BestLevel = rdr.ReadUInt16()
        );
        RegisterProperty("BestFame",
            (ref wtr) => wtr.Write(BestFame),
            (ref rdr) => BestFame = rdr.ReadUInt32()
        );
    }

    public override string Key => KEY_BASE + $".{Id}";

    public int Id { get; set; }

    public ushort ObjectType { get; set; }

    public ushort BestLevel { get; set; }

    public uint BestFame { get; set; }

    public int AccStatsId { get; set; }

    public virtual AccountStat? AccStats { get; set; }

    public static IEnumerable<string> GetIncludes() {
        yield break;
    }

    public static ClassStat Read(string key) {
        var ret = new ClassStat();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }

    public static string BuildKey(int id) {
        return KEY_BASE + $".{id}";
    }
}
using System;
using System.Collections.Generic;
using Common.Utilities;

namespace Common.Database.Models;

public class GuildMember : DbModel, IDbQueryable {
    public const string KEY_BASE = "guildMember";

    public GuildMember() {
        RegisterProperty("Id",
            (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("GuildRank",
            (ref wtr) => wtr.Write(GuildRank),
            (ref rdr) => GuildRank = rdr.ReadInt16()
        );
        RegisterProperty("LastSeenAt",
            (ref wtr) => wtr.Write(LastSeenAt.ToUnixTimestamp()),
            (ref rdr) => LastSeenAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
    }

    public override string Key => KEY_BASE + $".{Id}";

    public int Id { get; set; }

    public short GuildRank { get; set; }

    public DateTime LastSeenAt { get; set; }

    public int GuildId { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual Guild Guild { get; set; }

    public static IEnumerable<string> GetIncludes() {
        yield break;
    }

    public static GuildMember Read(string key) {
        var ret = new GuildMember();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }

    public static string BuildKey(int id) {
        return KEY_BASE + $".{id}";
    }
}
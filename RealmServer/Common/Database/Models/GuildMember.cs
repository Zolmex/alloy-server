using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class GuildMember : DbModel, IDbQueryable
{
    public const string KEY_BASE = "guildMember";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public short? GuildRank { get; set; }

    public DateTime? LastSeenAt { get; set; }

    public int GuildId { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual Guild? Guild { get; set; }

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("GuildRank",
            wtr => wtr.Write(GuildRank ?? 0),
            rdr => GuildRank = rdr.ReadInt16()
        );
        RegisterProperty("LastSeenAt",
            wtr => wtr.Write((LastSeenAt ?? DateTime.MinValue).ToUnixTimestamp()),
            rdr => LastSeenAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
    }

    public static GuildMember Read(string key)
    {
        var ret = new GuildMember();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }

    public static IEnumerable<string> GetIncludes()
    {
        yield break;
    }
    
    public static string BuildKey(int id)
    {
        return KEY_BASE + $".{id}";
    }
}

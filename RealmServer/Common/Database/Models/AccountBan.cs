using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountBan : DbModel, IDbQueryable
{
    public const string KEY_BASE = "accountBan";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public int BanType { get; set; }

    public string? Reason { get; set; }

    public DateTime BannedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int ModeratorId { get; set; }

    public int BannedId { get; set; }

    public virtual Account Banned { get; set; } = null!;

    public virtual Account Moderator { get; set; } = null!;
    
    public AccountBan()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("BanType",
            wtr => wtr.Write(BanType),
            rdr => BanType = rdr.ReadInt32()
        );
        RegisterProperty("Reason",
            wtr => wtr.Write(Reason ?? ""),
            rdr => Reason = rdr.ReadUTF()
        );
        RegisterProperty("BannedAt",
            wtr => wtr.Write(BannedAt.ToUnixTimestamp()),
            rdr => BannedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("ExpiresAt",
            wtr => wtr.Write((ExpiresAt ?? DateTime.MaxValue).ToUnixTimestamp()),
            rdr => ExpiresAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
    }
    
    public static AccountBan Read(string key)
    {
        var ret = new AccountBan();
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

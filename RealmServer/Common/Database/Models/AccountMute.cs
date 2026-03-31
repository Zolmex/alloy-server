using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountMute : DbModel, IDbQueryable
{
    public const string KEY_BASE = "accountMute";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public string? Reason { get; set; }

    public DateTime MutedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int ModeratorId { get; set; }

    public int MutedId { get; set; }

    public virtual Account Moderator { get; set; } = null!;

    public virtual Account Muted { get; set; } = null!;

    public AccountMute()
    {
        RegisterProperty("Id",
           (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("Reason",
           (ref wtr) => wtr.WriteUTF(Reason ?? ""),
            (ref rdr) => Reason = rdr.ReadUTF()
        );
        RegisterProperty("MutedAt",
           (ref wtr) => wtr.Write(MutedAt.ToUnixTimestamp()),
            (ref rdr) => MutedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("ExpiresAt",
           (ref wtr) => wtr.Write((ExpiresAt ?? DateTime.MaxValue).ToUnixTimestamp()),
            (ref rdr) => ExpiresAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
    }
    
    public static AccountMute Read(string key)
    {
        var ret = new AccountMute();
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

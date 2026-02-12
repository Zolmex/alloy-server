using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Login : DbModel, IDbQueryable
{
    public const string KEY_BASE = "login";
    
    public override string Key => KEY_BASE + $".{Id}";
    
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

    public DateTime? LastLoginAt { get; set; }
    public string? IPAddress { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public Login()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("Name",
            wtr => wtr.Write(Name),
            rdr => Name = rdr.ReadUTF()
        );
        RegisterProperty("PasswordHash",
            wtr => wtr.Write(PasswordHash ?? ""),
            rdr => PasswordHash = rdr.ReadUTF()
        );
        RegisterProperty("PasswordSalt",
            wtr => wtr.Write(PasswordSalt ?? ""),
            rdr => PasswordSalt = rdr.ReadUTF()
        );
        RegisterProperty("LastLoginAt",
            wtr => wtr.Write((LastLoginAt ?? DateTime.MinValue).ToUnixTimestamp()),
            rdr => LastLoginAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("IPAddress",
            wtr => wtr.Write(IPAddress ?? ""),
            rdr => IPAddress = rdr.ReadUTF()
        );
    }

    public static Login Read(string key)
    {
        var ret = new Login();
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

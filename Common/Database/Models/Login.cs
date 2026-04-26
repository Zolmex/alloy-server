using System;
using System.Collections.Generic;
using Common.Utilities;

namespace Common.Database.Models;

public class Login : DbModel, IDbQueryable {
    public const string KEY_BASE = "login";

    public Login() {
        RegisterProperty("Id",
            (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("Name",
            (ref wtr) => wtr.WriteUTF(Name),
            (ref rdr) => Name = rdr.ReadUTF()
        );
        RegisterProperty("PasswordHash",
            (ref wtr) => wtr.WriteUTF(PasswordHash ?? ""),
            (ref rdr) => PasswordHash = rdr.ReadUTF()
        );
        RegisterProperty("PasswordSalt",
            (ref wtr) => wtr.WriteUTF(PasswordSalt ?? ""),
            (ref rdr) => PasswordSalt = rdr.ReadUTF()
        );
        RegisterProperty("LastLoginAt",
            (ref wtr) => wtr.Write((LastLoginAt ?? DateTime.MinValue).ToUnixTimestamp()),
            (ref rdr) => LastLoginAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("IPAddress",
            (ref wtr) => wtr.WriteUTF(IPAddress ?? ""),
            (ref rdr) => IPAddress = rdr.ReadUTF()
        );
    }

    public override string Key => KEY_BASE + $".{Id}";

    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

    public DateTime? LastLoginAt { get; set; }
    public string? IPAddress { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public static IEnumerable<string> GetIncludes() {
        yield break;
    }

    public static Login Read(string key) {
        var ret = new Login();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }

    public static string BuildKey(int id) {
        return KEY_BASE + $".{id}";
    }
}
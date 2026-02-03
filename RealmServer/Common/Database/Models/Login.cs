using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Login : IDbModel
{
    public string Key => $"login.{Id}";
    
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

    public DateTime? LastLoginAt { get; set; }
    public string? IPAddress { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(Name);
        wtr.Write(PasswordHash ?? "");
        wtr.Write(PasswordSalt ?? "");
        wtr.Write((LastLoginAt ?? DateTime.MinValue).ToUnixTimestamp());
        wtr.Write(IPAddress ?? "");
    }

    public static Login Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;
        
        var ret = new Login();
        ret.Id = id;
        ret.Name = rdr.ReadUTF();
        ret.PasswordHash = rdr.ReadUTF();
        ret.PasswordSalt = rdr.ReadUTF();
        ret.LastLoginAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        ret.IPAddress = rdr.ReadUTF();
        return ret;
    }
}

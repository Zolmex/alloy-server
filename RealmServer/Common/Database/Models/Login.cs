using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Login
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

    public DateTime? LastLoginAt { get; set; }
    public string? IPAddress { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)1);
        
        wtr.Write(Id);
        wtr.Write(Name);
        wtr.Write(PasswordHash ?? "");
        wtr.Write(PasswordSalt ?? "");
        wtr.Write((LastLoginAt ?? DateTime.MinValue).ToUnixTimestamp());
        wtr.Write(IPAddress ?? "");
    }

    public static Login Read(NetworkReader rdr)
    {
        if (rdr.ReadByte() == 0) // Empty flag
            return null;
        
        var ret = new Login();
        ret.Id = rdr.ReadInt32();
        ret.Name = rdr.ReadUTF();
        ret.PasswordHash = rdr.ReadUTF();
        ret.PasswordSalt = rdr.ReadUTF();
        ret.LastLoginAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        ret.IPAddress = rdr.ReadUTF();
        return ret;
    }
}

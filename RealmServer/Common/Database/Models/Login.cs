using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Login : IDbSerializable
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
        wtr.Write(Id);
        wtr.Write(Name);
        wtr.Write(PasswordHash!);
        wtr.Write(PasswordSalt!);
        wtr.Write(LastLoginAt!.Value.ToUnixTimestamp());
        wtr.Write(IPAddress!);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new Login()
        {
            Id = rdr.ReadInt32(),
            Name = rdr.ReadUTF(),
            PasswordHash = rdr.ReadUTF(),
            PasswordSalt = rdr.ReadUTF(),
            LastLoginAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32()),
            IPAddress = rdr.ReadUTF()
        };
    }
}

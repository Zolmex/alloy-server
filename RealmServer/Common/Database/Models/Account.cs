using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Account : IDbSerializable
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public short? Rank { get; set; }

    public string? GuildName { get; set; }

    public bool? IsAdmin { get; set; }

    public bool? IsBanned { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? AccStatsId { get; set; }

    public int? LoginId { get; set; }
    
    public short? MaxChars { get; set; }
    
    public short? VaultCount { get; set; }

    public virtual AccountStat? AccStats { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public virtual ICollection<GuildMember> GuildMembers { get; set; } = new List<GuildMember>();

    public virtual Login? Login { get; set; }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(Name);
        wtr.Write(Rank!.Value);
        wtr.Write(GuildName!);
        wtr.Write(IsAdmin!.Value);
        wtr.Write(IsBanned!.Value);
        wtr.Write(CreatedAt!.Value.ToUnixTimestamp());
        wtr.Write(AccStatsId!.Value);
        wtr.Write(LoginId!.Value);
        wtr.Write(MaxChars!.Value);
        wtr.Write(VaultCount!.Value);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new Account()
        {
            Id = rdr.ReadInt32(),
            Name = rdr.ReadString(),
            Rank = rdr.ReadInt16(),
            GuildName = rdr.ReadUTF(),
            IsAdmin = rdr.ReadBoolean(),
            IsBanned = rdr.ReadBoolean(),
            CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32()),
            AccStatsId = rdr.ReadInt32(),
            LoginId = rdr.ReadInt32(),
            MaxChars = rdr.ReadInt16(),
            VaultCount = rdr.ReadInt16()
        };
    }
}

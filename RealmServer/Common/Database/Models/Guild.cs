using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Guild : IDbSerializable
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public short? Level { get; set; }

    public uint? CurrentFame { get; set; }

    public uint? TotalFame { get; set; }

    public string? GuildBoard { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<GuildMember> GuildMembers { get; set; } = new List<GuildMember>();
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(Name!);
        wtr.Write(Level!.Value);
        wtr.Write(CurrentFame!.Value);
        wtr.Write(TotalFame!.Value);
        wtr.Write(GuildBoard!);
        wtr.Write(CreatedAt!.Value.ToUnixTimestamp());
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new Guild()
        {
            Id = rdr.ReadInt32(),
            Name = rdr.ReadUTF(),
            Level = rdr.ReadInt16(),
            CurrentFame = rdr.ReadUInt32(),
            TotalFame = rdr.ReadUInt32(),
            GuildBoard = rdr.ReadUTF(),
            CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        };
    }
}

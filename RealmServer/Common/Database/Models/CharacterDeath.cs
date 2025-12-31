using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterDeath : IDbSerializable
{
    public int Id { get; set; }

    public DateTime? DeadAt { get; set; }

    public uint? DeathFame { get; set; }

    public int? CharId { get; set; }

    public virtual Character? Char { get; set; }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(DeadAt!.Value.ToUnixTimestamp());
        wtr.Write(DeathFame!.Value);
        wtr.Write(CharId!.Value);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new CharacterDeath()
        {
            Id = rdr.ReadInt32(),
            DeadAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32()),
            DeathFame = rdr.ReadUInt32(),
            CharId = rdr.ReadInt32()
        };
    }
}

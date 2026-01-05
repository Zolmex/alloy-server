using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterDeath
{
    public int Id { get; set; }

    public DateTime? DeadAt { get; set; }

    public uint? DeathFame { get; set; }

    public int? CharId { get; set; }

    public virtual Character? Char { get; set; }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)1);
        
        wtr.Write(Id);
        wtr.Write((DeadAt ?? DateTime.MinValue).ToUnixTimestamp());
        wtr.Write(DeathFame ?? 0);
        wtr.Write(CharId ?? 0);
    }

    public static CharacterDeath Read(NetworkReader rdr)
    {
        if (rdr.ReadByte() == 0) // Empty flag
            return null;
        
        var ret = new CharacterDeath();
        ret.Id = rdr.ReadInt32();
        ret.DeadAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        ret.DeathFame = rdr.ReadUInt32();
        ret.CharId = rdr.ReadInt32();
        return ret;
    }
}

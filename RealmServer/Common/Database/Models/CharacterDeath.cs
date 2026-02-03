using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterDeath : IDbModel
{
    public string Key => $"characterDeath.{Id}";
    
    public int Id { get; set; }

    public DateTime? DeadAt { get; set; }

    public uint? DeathFame { get; set; }

    public int? CharId { get; set; }

    public virtual Character? Char { get; set; }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write((DeadAt ?? DateTime.MinValue).ToUnixTimestamp());
        wtr.Write(DeathFame ?? 0);
        if (Char != null)
            Char.Write(wtr);
        else wtr.Write(0);
    }

    public static CharacterDeath Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;
        
        var ret = new CharacterDeath();
        ret.Id = id;
        ret.DeadAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        ret.DeathFame = rdr.ReadUInt32();
        ret.Char = Character.Read(rdr);
        ret.CharId = ret.Char?.Id ?? 0;
        return ret;
    }
}

using Common.Network;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterDeath : DbModel
{
    public override string Key => $"characterDeath.{Id}";
    
    public int Id { get; set; }

    public DateTime? DeadAt { get; set; }

    public uint? DeathFame { get; set; }

    public int? CharId { get; set; }

    public virtual Character? Char { get; set; }

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("DeadAt",
            wtr => wtr.Write((DeadAt ?? DateTime.MinValue).ToUnixTimestamp()),
            rdr => DeadAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("DeathFame",
            wtr => wtr.Write(DeathFame ?? 0),
            rdr => DeathFame = rdr.ReadUInt32()
        );
        RegisterProperty("Char",
            wtr =>
            {
                var hasValue = Char != null;
                wtr.Write(hasValue);
                if (hasValue)
                    Char.WriteProperties(wtr);
            },
            rdr =>
            {
                Char = DbModel.Read<Character>(rdr);
                CharId = Char?.Id ?? 0;
            }
        );
    }
    
    public static CharacterDeath Read(string key)
    {
        var ret = new CharacterDeath();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }
}

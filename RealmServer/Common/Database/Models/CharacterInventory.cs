using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterInventory
{
    public int CharacterId { get; set; }

    public int SlotId { get; set; }

    public ushort? ItemType { get; set; }

    public byte[]? ItemData { get; set; }

    public virtual Character Character { get; set; } = null!;
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(CharacterId);
        wtr.Write(SlotId);
        wtr.Write(ItemType ?? 0);
        wtr.Write(ItemData ?? []);
    }

    public static CharacterInventory Read(NetworkReader rdr)
    {
        var ret = new CharacterInventory();
        ret.CharacterId = rdr.ReadInt32();
        ret.SlotId = rdr.ReadInt32();
        ret.ItemType = rdr.ReadUInt16();
        ret.ItemData = rdr.Read<byte>();
        return ret;
    }
}

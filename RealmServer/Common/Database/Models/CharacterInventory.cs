using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterInventory : IDbSerializable
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
        wtr.Write(ItemType!.Value);
        wtr.Write(ItemData);
    }

    public IDbSerializable Read(NetworkReader rdr)
    {
        return new CharacterInventory()
        {
            CharacterId = rdr.ReadInt32(),
            SlotId =  rdr.ReadInt32(),
            ItemType = rdr.ReadUInt16(),
            ItemData = rdr.Read<byte>()
        };
    }
}

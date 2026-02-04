using Common.Network;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class CharacterInventory : DbModel
{
    public override string Key => $"characterInventory.{CharacterId}.{SlotId}";
    
    public int CharacterId { get; set; }

    public int SlotId { get; set; }

    public ushort? ItemType { get; set; }

    public byte[]? ItemData { get; set; }

    public virtual Character Character { get; set; } = null!;

    protected override void Prepare()
    {
        RegisterProperty("CharacterId",
            wtr => wtr.Write(CharacterId),
            rdr => CharacterId = rdr.ReadInt32()
        );
        RegisterProperty("SlotId",
            wtr => wtr.Write(SlotId),
            rdr => SlotId = rdr.ReadInt32()
        );
        RegisterProperty("ItemType",
            wtr => wtr.Write(ItemType ?? 0),
            rdr => ItemType = rdr.ReadUInt16()
        );
        RegisterProperty("ItemData",
            wtr => wtr.Write(ItemData ?? []),
            rdr => ItemData = rdr.Read<byte>()
        );
    }

    public static CharacterInventory Read(string key)
    {
        var ret = new CharacterInventory();
        var split = key.Split('.');
        ret.CharacterId = int.Parse(split[1]);
        ret.SlotId = int.Parse(split[2]);
        return ret;
    }
}

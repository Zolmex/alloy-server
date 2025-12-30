using System;
using System.Collections.Generic;

namespace DbServer.Database;

public partial class CharacterInventory
{
    public int CharacterId { get; set; }

    public int SlotId { get; set; }

    public ushort? ItemType { get; set; }

    public byte[]? ItemData { get; set; }

    public virtual Character Character { get; set; } = null!;
}

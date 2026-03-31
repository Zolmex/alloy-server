using Common.Network;
using System.IO;

namespace Common;

public struct TradeItem
{
    public int Item;
    public ItemType SlotType;
    public bool Tradeable;
    public bool Included;

    public void Write(ref SpanWriter wtr)
    {
        wtr.Write(Item);
        wtr.Write((int)SlotType);
        wtr.Write(Tradeable);
        wtr.Write(Included);
    }
}
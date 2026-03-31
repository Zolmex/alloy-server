using Common.Network;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct AccountList(int AccountListId, int[] AccountIDs) : IOutgoingPacket
{
    public PacketId ID => PacketId.ACCOUNTLIST;
    
    public const int Locked = 0;
    public const int Ignored = 1;

    public void Write(ref SpanWriter wtr)
    {
        wtr.Write(AccountListId);
        wtr.Write(AccountIDs);
    }
}
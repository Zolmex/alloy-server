namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct AccountList(int AccountListId, int[] AccountIDs) : IOutgoingPacket
{
    public PacketId ID => PacketId.ACCOUNTLIST;
    
    public const int Locked = 0;
    public const int Ignored = 1;
}
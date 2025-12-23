#region

#endregion

using Common.Utilities.Net;

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct AccountList(int AccountListId, int[] AccountIDs) : IOutgoingPacket<AccountList>
{
    public const int Locked = 0;
    public const int Ignored = 1;
}
#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly record struct AccountList : IOutgoingPacket
{
    public const int Locked = 0;
    public const int Ignored = 1;

    public static PacketId PacketId => PacketId.ACCOUNTLIST;

    public int AccountListId { get; }
    public int[] AccountIds { get; }

    public static void Write(NetworkHandler network, int accListId, int[] accIds)
    {
        var state = network.SendState;
        var wtr = state.Writer;
        using (TimedLock.Lock(state))
        {
            var begin = state.PacketBegin();

            wtr.Write(accListId);
            wtr.Write((short)accIds.Length);
            foreach (var id in accIds)
                wtr.Write(id);

            state.PacketEnd(begin, PacketId.ACCOUNTLIST);
        }
    }

    public override string ToString()
    {
        var type = typeof(AccountList);
        var props = type.GetProperties();
        var ret = $"\n";
        foreach (var prop in props)
        {
            ret += $"{prop.Name}:{prop.GetValue(this)}";
            if (!(props.IndexOf(prop) == props.Length - 1))
                ret += "\n";
        }

        return ret;
    }
}
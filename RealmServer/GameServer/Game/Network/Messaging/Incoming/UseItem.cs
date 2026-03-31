#region

using Common;
using Common.Network;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.USEITEM)]
public partial record UseItem : IIncomingPacket
{
    public SlotObjectData Slot;
    public int Time;
    public WorldPosData UsePos;

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        user.GameInfo.Player.UseItem(Slot, UsePos, Time);
    }

    public void Read(ref SpanReader rdr)
    {
        Slot = rdr.ReadSlotObjectData();
        UsePos = WorldPosData.Read(ref rdr);
        Time = rdr.ReadInt32();
    }
}
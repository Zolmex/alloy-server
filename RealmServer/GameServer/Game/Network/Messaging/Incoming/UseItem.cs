#region

using Common;
using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.USEITEM)]
    public partial record UseItem : IIncomingPacket
    {
        public SlotObjectData Slot;
        public WorldPosData UsePos;
        public int Time;

        public void Read(NetworkReader rdr)
        {
            Slot = rdr.ReadSlotObjectData();
            UsePos = WorldPosData.Read(rdr);
            Time = rdr.ReadInt32();
        }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            user.GameInfo.Player.UseItem(Slot, UsePos, Time);
        }
    }
}
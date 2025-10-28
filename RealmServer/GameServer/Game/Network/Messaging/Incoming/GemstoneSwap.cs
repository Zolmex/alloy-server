#region

using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.GEMSTONE_SWAP)]
    public class GemstoneSwap : IIncomingPacket
    {
        public byte Slot;
        public byte GemSlot;
        public byte InvSlot;


        public void Read(NetworkReader rdr)
        {
            Slot = rdr.ReadByte();
            GemSlot = rdr.ReadByte();
            InvSlot = rdr.ReadByte();
        }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            var player = user.GameInfo.Player;
            player.Inventory.SwapGemstones(Slot, GemSlot, InvSlot);
        }

        public override string ToString()
        {
            var type = typeof(GemstoneSwap);
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
}
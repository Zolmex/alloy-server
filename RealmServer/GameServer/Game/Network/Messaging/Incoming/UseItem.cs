#region

using Common;
using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.USEITEM)]
    public class UseItem : IIncomingPacket
    {
        public SlotObjectData Slot;
        public WorldPosData UsePos;
        public int Time;

        public void Read(NetworkReader rdr)
        {
            Slot = SlotObjectData.Read(rdr);
            UsePos = WorldPosData.Read(rdr);
            Time = rdr.ReadInt32();
        }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            user.GameInfo.Player.UseItem(Slot, UsePos, Time);
        }

        public override string ToString()
        {
            var type = typeof(UseItem);
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
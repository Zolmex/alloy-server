#region

using Common;
using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.SERVERPLAYERSHOOT)]
    public class ServerPlayerShoot : IOutgoingPacket
    {
        public WorldPosData StartPos { get; }
        public float Angle { get; }
        public float AngleInc { get; }
        public int[] DamageList { get; }
        public float[] CritList { get; }
        public short ItemType { get; }

        public static void Write(NetworkHandler network, WorldPosData startPos,
            float angle, float angleInc, int[] damageList, float[] critList, int itemType = -1)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                startPos.Write(wtr);
                wtr.Write(angle);
                wtr.Write(angleInc);

                wtr.Write((byte)damageList.Length);
                for (var i = 0; i < damageList.Length; i++)
                    wtr.Write(damageList[i]);

                wtr.Write((byte)critList.Length);
                for (var i = 0; i < critList.Length; i++)
                    wtr.Write(critList[i]);

                wtr.Write((short)itemType);

                state.PacketEnd(begin, PacketId.SERVERPLAYERSHOOT);
            }
        }

        public override string ToString()
        {
            var type = typeof(ServerPlayerShoot);
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
#region

using Common;
using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.SHOWEFFECT)]
    public class ShowEffect : IOutgoingPacket
    {
        public byte EffectType { get; }
        public int TargetId { get; }
        public int Color { get; }
        public float EffectParam { get; }
        public WorldPosData Pos1 { get; }
        public WorldPosData Pos2 { get; }

        public static void Write(NetworkHandler network, byte effectType, int targetId, int color, float effectParam, WorldPosData pos1, WorldPosData pos2)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(effectType);
                wtr.Write(targetId);
                wtr.Write(color);
                wtr.Write(effectParam);
                pos1.Write(wtr);
                pos2.Write(wtr);

                state.PacketEnd(begin, PacketId.SHOWEFFECT);
            }
        }

        public override string ToString()
        {
            var type = typeof(ShowEffect);
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
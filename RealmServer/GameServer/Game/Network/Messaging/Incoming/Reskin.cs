#region

using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.RESKIN)]
    public class Reskin : IIncomingPacket
    {
        public int SkinType;

        public void Read(NetworkReader rdr)
        {
            SkinType = rdr.ReadInt32();
        }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            user.GameInfo.Player.Skin = SkinType;
        }

        public override string ToString()
        {
            var type = typeof(Reskin);
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
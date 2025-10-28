#region

using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.PLAYERTEXT)]
    public class PlayerText : IIncomingPacket
    {
        public string Text;

        public void Read(NetworkReader rdr)
        {
            Text = rdr.ReadUTF();
        }

        public void Handle(User user)
        {
            if (user.Account.Muted)
            {
                user.GameInfo.Player.SendError("You are muted.");
                return;
            }

            user.GameInfo.Player.Speak(Text);
        }

        public override string ToString()
        {
            var type = typeof(PlayerText);
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
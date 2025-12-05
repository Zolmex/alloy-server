#region

using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.PLAYERTEXT)]
    public partial record PlayerText() : IIncomingPacket
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
    }
}
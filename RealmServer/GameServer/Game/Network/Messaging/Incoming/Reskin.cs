#region

using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.RESKIN)]
    public partial record Reskin : IIncomingPacket
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
    }
}
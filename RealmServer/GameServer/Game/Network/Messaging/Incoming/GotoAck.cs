#region

using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.GOTOACK)]
    public partial record GotoAck : IIncomingPacket
    {
        public void Read(NetworkReader rdr)
        { }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            var plr = user.GameInfo.Player;
            if (!plr.Teleporting)
                return;

            plr.FinishTeleport();
        }
    }
}
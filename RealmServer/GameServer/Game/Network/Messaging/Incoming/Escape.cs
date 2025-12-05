#region

using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.Worlds;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.ESCAPE)]
    public partial record Escape : IIncomingPacket
    {
        public void Read(NetworkReader rdr)
        { }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            if (user.GameInfo.World.Id == World.NEXUS_ID)
            {
                user.GameInfo.Player.SendInfo("You're already in the Nexus!");
                return;
            }

            user.ReconnectTo(RealmManager.NexusInstance);
        }
    }
}
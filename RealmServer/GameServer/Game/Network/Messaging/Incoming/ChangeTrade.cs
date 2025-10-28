#region

using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.CHANGETRADE)]
    public class ChangeTrade : IIncomingPacket
    {
        public bool[] Offer;

        public void Read(NetworkReader rdr)
        {
            Offer = new bool[rdr.ReadByte()];
            for (var i = 0; i < Offer.Length; i++)
                Offer[i] = rdr.ReadBoolean();
        }

        public void Handle(User user)
        {
            var player = user.GameInfo.Player;
            if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
                return;

            player.ChangeTrade(Offer);
        }
    }
}
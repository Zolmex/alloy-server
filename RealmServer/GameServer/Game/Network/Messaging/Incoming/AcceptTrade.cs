#region

using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.ACCEPTTRADE)]
    public class AcceptTrade : IIncomingPacket
    {
        public bool[] MyOffer;
        public bool[] TheirOffer;

        public void Read(NetworkReader rdr)
        {
            MyOffer = new bool[rdr.ReadByte()];
            for (var i = 0; i < MyOffer.Length; i++)
                MyOffer[i] = rdr.ReadBoolean();
            TheirOffer = new bool[rdr.ReadByte()];
            for (var i = 0; i < TheirOffer.Length; i++)
                TheirOffer[i] = rdr.ReadBoolean();
        }

        public void Handle(User user)
        {
            var player = user.GameInfo.Player;
            if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
                return;

            player.AcceptTrade(MyOffer, TheirOffer);
        }
    }
}
#region

using Common;
using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.MOVE)]
    public partial record Move : IIncomingPacket
    {
        public WorldPosData Pos;

        public void Read(NetworkReader rdr)
        {
            Pos = WorldPosData.Read(rdr);
        }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            var player = user.GameInfo.Player;
            if (!player.ValidateMove(Pos.X, Pos.Y))
                return;

            player.Move(Pos.X, Pos.Y);
            player.LastMoveAck = RealmManager.RealTime.ElapsedMilliseconds; // Setting the last move ack here allows the new position to be valid at one point (e.g. the server lagged)
        }
    }
}
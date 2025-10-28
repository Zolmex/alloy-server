#region

using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging
{
    public interface IIncomingPacket
    {
        public void Read(NetworkReader rdr);
        public void Handle(User user);
    }
}
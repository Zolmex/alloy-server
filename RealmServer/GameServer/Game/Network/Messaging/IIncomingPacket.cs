#region

using Common.Network;

#endregion

namespace GameServer.Game.Network.Messaging;

public interface IIncomingPacket
{
    void Read(NetworkReader rdr);
    void Handle(User user);
}
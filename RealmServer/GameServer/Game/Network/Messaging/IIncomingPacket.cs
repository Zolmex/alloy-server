#region

using Common.Network;

#endregion

namespace GameServer.Game.Network.Messaging;

public interface IIncomingPacket {
    void Read(ref SpanReader rdr);
    void Handle(User user);
}
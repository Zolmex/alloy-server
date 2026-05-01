#region

using Common.Network;

#endregion

namespace GameServer.Game.Network.Messaging;

public interface IIncomingPacket {
    void Read(ref SpanReader rdr);
    Task Handle(User user);
}
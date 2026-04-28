#region

using Common.Network;

#endregion

namespace GameServerOld.Game.Network.Messaging;

public interface IIncomingPacket {
    void Read(ref SpanReader rdr);
    void Handle(User user);
}
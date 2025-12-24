using Common.Network;

namespace GameServer.Game.Network.Messaging;

public interface IOutgoingPacket<TSelf> where TSelf : IOutgoingPacket<TSelf>
{
    static abstract PacketId PacketId { get; }
    static abstract TSelf Read(NetworkReader rdr);
    void Write(NetworkWriter wtr);
}
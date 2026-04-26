using Common.Network.Messaging;

namespace GameServer.Game.Network.Messaging;

public interface IOutgoingPacket : IWritable {
    PacketId ID { get; }
}
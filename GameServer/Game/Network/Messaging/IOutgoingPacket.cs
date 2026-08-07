using Common.Network;

namespace GameServer.Game.Network.Messaging;

public interface IOutgoingPacket : IWritable {
    PacketId ID { get; }
}
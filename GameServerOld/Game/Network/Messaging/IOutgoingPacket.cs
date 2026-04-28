using Common.Network.Messaging;

namespace GameServerOld.Game.Network.Messaging;

public interface IOutgoingPacket : IWritable {
    PacketId ID { get; }
}
using Common.Utilities.Net;

namespace GameServer.Game.Network.Messaging;

public interface IOutgoingPacket
{
    public static abstract PacketId PacketId { get; }
    public void Write(NetworkWriter wtr);
}
using Common.Utilities.Net;

namespace GameServer.Game.Network.Messaging;

public interface IOutgoingPacket<T> where T : IOutgoingPacket<T>
{
    public static abstract PacketId PacketId { get; }
    public static abstract T Read(NetworkReader rdr);
    public void Write(NetworkWriter wtr);
}
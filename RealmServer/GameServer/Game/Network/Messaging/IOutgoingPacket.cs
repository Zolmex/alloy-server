using Common.Utilities.Net;

namespace GameServer.Game.Network.Messaging;

public interface IOutgoingPacket<TSelf> where TSelf : IOutgoingPacket<TSelf>
{
    public static abstract PacketId PacketId { get; }
    public static abstract TSelf Read(NetworkReader rdr);
    public void Write(NetworkWriter wtr);
}
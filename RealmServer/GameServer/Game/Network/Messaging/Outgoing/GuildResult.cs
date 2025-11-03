#region

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct GuildResult(bool Success, string ErrorText) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.GUILDRESULT;
    public const string SUCCESS = "Success!";
}
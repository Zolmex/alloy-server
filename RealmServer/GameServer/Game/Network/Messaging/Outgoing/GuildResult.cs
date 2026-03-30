namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct GuildResult(bool Success, string ErrorText) : IOutgoingPacket
{
    public PacketId ID => PacketId.GUILDRESULT;
    
    public const string SUCCESS = "Success!";
}
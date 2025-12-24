namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct GuildResult(bool Success, string ErrorText) : IOutgoingPacket<GuildResult>
{
    public const string SUCCESS = "Success!";
}
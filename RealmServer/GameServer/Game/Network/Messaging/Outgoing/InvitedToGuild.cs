namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct InvitedToGuild(string PlayerName, string GuildName) : IOutgoingPacket<InvitedToGuild>
{ }